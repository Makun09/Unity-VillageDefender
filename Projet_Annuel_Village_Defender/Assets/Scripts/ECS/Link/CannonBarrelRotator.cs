using System;
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
    /// Fait pivoter le barillet/corps du canon vers le gobelin le plus proche
    /// en portée, en utilisant la même logique de ciblage que TowerFireSystem :
    /// query ECS des gobelins vivants → distance → plus proche dans la portée.
    ///
    /// À placer sur le child du prefab qui doit physiquement tourner.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class CannonBarrelRotator : MonoBehaviour
    {
        /// <summary>
        /// Axes de rotation autorisés. Combinables dans l'Inspector (flags).
        /// </summary>
        [Flags]
        public enum RotationAxes
        {
            X   = 1,
            Y   = 2,
            Z   = 4,
            XY  = X | Y,
            XZ  = X | Z,
            YZ  = Y | Z,
            XYZ = X | Y | Z,
        }

        [Tooltip("Axe(s) sur lesquels ce GameObject peut pivoter vers la cible.")]
        [SerializeField] private RotationAxes rotationAxes = RotationAxes.Y;

        [Tooltip("Vitesse de rotation en degrés par seconde.")]
        [SerializeField] private float rotationSpeed = 360f;

        [Header("Offset de rotation")]
        [Tooltip("Angle de correction appliqué après la rotation vers la cible (X=tangage, Y=lacet, Z=roulis).")]
        [SerializeField] private Vector3 rotationOffset = Vector3.zero;

        private BuildingEntityLink _link;
        private EntityQuery        _goblinQuery;
        private bool               _queryReady;
        private World              _queryWorld;

        private void OnDestroy()
        {
            SafeDisposeGoblinQuery();
        }

        private void LateUpdate()
        {
            if (_link == null) _link = GetComponentInParent<BuildingEntityLink>();
            if (_link == null) return;

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated) return;
            var em = world.EntityManager;

            if (!em.Exists(_link.LinkedEntity)) return;
            if (!em.HasComponent<TowerAttack>(_link.LinkedEntity)) return;

            var attack  = em.GetComponentData<TowerAttack>(_link.LinkedEntity);
            var rangeSq = attack.Range * attack.Range;
            var root    = _link.transform.position;
            var towerPos = new float3(root.x, root.y, root.z);

            // Recrée la query si le World ECS a changé (restart/reset).
            if (_queryReady && _queryWorld != world)
            {
                SafeDisposeGoblinQuery();
            }

            // ── Construction de la query gobelin (une seule fois par World) ────
            if (!_queryReady)
            {
                _goblinQuery = em.CreateEntityQuery(new EntityQueryDesc
                {
                    All  = new ComponentType[]
                    {
                        ComponentType.ReadOnly<LocalTransform>(),
                        ComponentType.ReadOnly<GoblinHealth>()
                    },
                    None = new ComponentType[]
                    {
                        ComponentType.ReadOnly<Prefab>(),
                        ComponentType.ReadOnly<Disabled>()
                    }
                });
                _queryReady = true;
                _queryWorld = world;
            }

            // ── Même logique que TowerFireJob : gobelin le plus proche en portée ─
            var xforms  = _goblinQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
            var healths = _goblinQuery.ToComponentDataArray<GoblinHealth>(Allocator.Temp);

            var nearestDistSq = float.MaxValue;
            var nearestPos    = float3.zero;
            var found         = false;

            for (var i = 0; i < xforms.Length; i++)
            {
                if (healths[i].Value <= 0f) continue;
                var gp  = xforms[i].Position;
                var dSq = math.distancesq(towerPos, gp);
                if (dSq > rangeSq) continue;
                if (dSq < nearestDistSq)
                {
                    nearestDistSq = dSq;
                    nearestPos    = gp;
                    found         = true;
                }
            }

            xforms.Dispose();
            healths.Dispose();

            if (!found) return;

            var dir       = math.normalizesafe(nearestPos - towerPos);
            var targetRot = ComputeTargetRotation(new Vector3(dir.x, dir.y, dir.z));
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        private Quaternion ComputeTargetRotation(Vector3 dir)
        {
            if (dir.sqrMagnitude < 0.001f) return transform.rotation;

            var fullEuler    = Quaternion.LookRotation(dir).eulerAngles;
            var currentEuler = transform.eulerAngles;

            // Applique seulement les axes cochés + l'offset de correction
            return Quaternion.Euler(
                ((rotationAxes & RotationAxes.X) != 0 ? fullEuler.x : currentEuler.x) + rotationOffset.x,
                ((rotationAxes & RotationAxes.Y) != 0 ? fullEuler.y : currentEuler.y) + rotationOffset.y,
                ((rotationAxes & RotationAxes.Z) != 0 ? fullEuler.z : currentEuler.z) + rotationOffset.z);
        }

        private void SafeDisposeGoblinQuery()
        {
            if (!_queryReady) return;

            try
            {
                _goblinQuery.Dispose();
            }
            catch (InvalidOperationException)
            {
                // Peut arriver si le World a deja ete detruit avant ce MonoBehaviour.
            }
            catch (NullReferenceException)
            {
                // Defensive: evite un crash pendant l'ordre de destruction ECS/MonoBehaviour.
            }
            finally
            {
                _goblinQuery = default;
                _queryReady = false;
                _queryWorld = null;
            }
        }
    }
}
