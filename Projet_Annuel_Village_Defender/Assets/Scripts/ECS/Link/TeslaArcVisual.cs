using System.Collections.Generic;
using ECS.Components.Building;
using ECS.Components.Enemy.SimpleGoblin;
using ECS.Link;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Visual
{
    /// <summary>
    /// Attach this to the Tesla-ball child GameObject of every Tesla tower prefab.
    ///
    /// Queries ECS directly each frame for goblin positions — does NOT depend on
    /// the TeslaCurrentTarget buffer, removing any job-scheduling race condition.
    ///
    /// Two render layers active simultaneously:
    ///   * Debug.DrawLine : works in ALL render pipelines; visible with Gizmos ON.
    ///   * LineRenderer   : real in-game line; assign a URP Unlit / Additive material.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TeslaArcVisual : MonoBehaviour
    {
        [Header("Visuel de l arc")]
        [Tooltip("Point d origine de l eclair (laisser vide = ce GameObject).")]
        [SerializeField] private Transform arcOrigin;

        [Tooltip("Materiau URP Unlit ou Additive. Si vide, un materiau de secours est cree.")]
        [SerializeField] private Material arcMaterial;

        [Tooltip("Couleur de l eclair.")]
        [SerializeField] private Color arcColor = new Color(0.35f, 0.85f, 1f, 1f);

        [Tooltip("Largeur de la ligne (world-units).")]
        [SerializeField] private float arcWidth = 0.08f;

        [Tooltip("Nombre de segments du zigzag.")]
        [Range(3, 24)]
        [SerializeField] private int segmentCount = 8;

        [Tooltip("Amplitude du bruit perpendiculaire.")]
        [SerializeField] private float noiseAmplitude = 0.3f;

        [Tooltip("Vitesse de scintillement.")]
        [SerializeField] private float flickerSpeed = 18f;

        private BuildingEntityLink _link;
        private readonly List<LineRenderer> _linePool = new();
        private EntityQuery _goblinQuery;
        private bool _queryReady;
        private float _time;

        private void OnDisable()
        {
            foreach (var lr in _linePool)
                if (lr != null) lr.enabled = false;
        }

        private void OnDestroy()
        {
            if (_queryReady)
                _goblinQuery.Dispose();
        }

        private void LateUpdate()
        {
            _time += Time.deltaTime;
            foreach (var lr in _linePool) lr.enabled = false;

            if (_link == null) _link = GetComponentInParent<BuildingEntityLink>();
            if (_link == null) return;

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated) return;
            var em = world.EntityManager;

            if (!em.Exists(_link.LinkedEntity)) return;
            if (!em.HasComponent<TowerAttack>(_link.LinkedEntity)) return;

            var attack     = em.GetComponentData<TowerAttack>(_link.LinkedEntity);
            var range      = attack.Range;
            var maxTargets = math.max(1, attack.FireCount);
            var originTransform = arcOrigin != null ? arcOrigin : transform;
            // Utilise le centre des bounds du mesh si dispo (pivot != centre visuel)
            var originRenderer  = originTransform.GetComponent<Renderer>();
            var towerPos        = originRenderer != null
                ? originRenderer.bounds.center
                : originTransform.position;

            if (!_queryReady)
            {
                _goblinQuery = em.CreateEntityQuery(new EntityQueryDesc
                {
                    All  = new ComponentType[] {
                        ComponentType.ReadOnly<LocalTransform>(),
                        ComponentType.ReadOnly<GoblinHealth>()
                    },
                    None = new ComponentType[] {
                        ComponentType.ReadOnly<Prefab>(),
                        ComponentType.ReadOnly<Disabled>()
                    }
                });
                _queryReady = true;
            }

            var goblinXforms  = _goblinQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
            var goblinHealths = _goblinQuery.ToComponentDataArray<GoblinHealth>(Allocator.Temp);

            var candidates = new List<(Vector3 pos, float distSq)>(goblinXforms.Length);
            for (var i = 0; i < goblinXforms.Length; i++)
            {
                if (goblinHealths[i].Value <= 0f) continue;
                var gp   = new Vector3(goblinXforms[i].Position.x, goblinXforms[i].Position.y, goblinXforms[i].Position.z);
                var dSq  = (gp - towerPos).sqrMagnitude;
                if (dSq <= range * range) candidates.Add((gp, dSq));
            }

            goblinXforms.Dispose();
            goblinHealths.Dispose();

            candidates.Sort((a, b) => a.distSq.CompareTo(b.distSq));

            var arcCount = math.min(candidates.Count, maxTargets);
            for (var i = 0; i < arcCount; i++)
            {
                var tp = candidates[i].pos;
                Debug.DrawLine(towerPos, tp, arcColor);
                var lr = GetOrCreateLine(i);
                lr.enabled = true;
                DrawArc(lr, towerPos, new float3(tp.x, tp.y, tp.z), i);
            }
        }

        private LineRenderer GetOrCreateLine(int index)
        {
            while (_linePool.Count <= index)
            {
                var go = new GameObject($"TeslaArc_{_linePool.Count}");
                go.transform.SetParent(transform, worldPositionStays: false);
                var lr = go.AddComponent<LineRenderer>();
                lr.useWorldSpace     = true;
                lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                lr.receiveShadows    = false;
                lr.positionCount     = segmentCount + 1;
                lr.startWidth        = arcWidth;
                lr.endWidth          = arcWidth * 0.5f;
                lr.startColor        = arcColor;
                lr.endColor          = new Color(arcColor.r, arcColor.g, arcColor.b, 0f);
                lr.numCapVertices    = 2;
                lr.enabled           = false;
                lr.material          = BuildMaterial();
                _linePool.Add(lr);
            }
            return _linePool[index];
        }

        private Material BuildMaterial()
        {
            if (arcMaterial != null) return new Material(arcMaterial);
            foreach (var name in new[]
            {
                "Universal Render Pipeline/Particles/Unlit",
                "Universal Render Pipeline/Unlit",
                "Unlit/Color",
                "Sprites/Default",
                "Legacy Shaders/Particles/Additive"
            })
            {
                var s = Shader.Find(name);
                if (s == null) continue;
                var mat = new Material(s);
                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", arcColor);
                if (mat.HasProperty("_Color"))     mat.SetColor("_Color",     arcColor);
                return mat;
            }
            return new Material(Shader.Find("Hidden/InternalErrorShader"));
        }

        private void DrawArc(LineRenderer lr, Vector3 start, float3 endF, int lineIndex)
        {
            var end   = new Vector3(endF.x, endF.y, endF.z);
            var delta = end - start;
            var perp  = Vector3.Cross(delta, Vector3.up);
            if (perp.sqrMagnitude < 0.0001f) perp = Vector3.right; else perp.Normalize();
            lr.positionCount = segmentCount + 1;
            for (var s = 0; s <= segmentCount; s++)
            {
                var t        = (float)s / segmentCount;
                var pos      = Vector3.Lerp(start, end, t);
                var envelope = Mathf.Sin(t * Mathf.PI);
                var noise    = Mathf.PerlinNoise(t * 4f + _time * flickerSpeed,
                                                 lineIndex * 47.3f + _time * flickerSpeed * 0.7f) * 2f - 1f;
                pos += perp * (noise * noiseAmplitude * envelope);
                lr.SetPosition(s, pos);
            }
        }
    }
}
