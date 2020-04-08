using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

namespace Geekbrains
{
    public sealed class Bot : BaseObjectScene, ITargeted
    {
        public float Hp = 100;
        public Vision Vision;
        public Weapon Weapon; //todo с разным оружием

        [SerializeField] private float _stoppingDistanceForDetected = 5.0f;
        [SerializeField] private float _stoppingDistanceForPatroling = 1.5f;
        [SerializeField] private Affiliation _affiliationSide;

        private float _waitTime = 3.0f;
        private float _rotationSpeed = 120.0f;
        private float _distanceOffset = 0.1f;
        private bool _isResetingState;
        private StateBot _stateBot;
        private BodyBot _bodyBot;
        private HeadBot _headBot;
        private ArmBot _armBot;
        private FlagBot _flagBot;
        private DetectorBot _detectorBot;
        private Vector3 _point;
        private Vector3 _visionPointBoarder;
        private List<Transform> _targetsTransforms = new List<Transform>();

        public event Action<Bot> OnDieChange;
        public event Action<Transform> OnDeath = delegate { };

        public NavMeshAgent Agent { get; private set; }
        [ShowInInspector] public Transform Target { get; set; }
        [ShowInInspector]
        public Affiliation AffiliationSide
        {
            get => _affiliationSide;
            set
            {
                _affiliationSide = value;
                SetSide();
            }
        }


        [ShowInInspector] private StateBot StateBot
        {
            get => _stateBot;
            set
            {
                _stateBot = value;
                switch (value)
                {
                    case StateBot.None:
                        //CustomDebug.Log($"Agent.hasPath = {Agent.hasPath}");
                        Color = Color.white;
                        break;
                    case StateBot.Patrol:
                        Color = Color.green;
                        break;
                    case StateBot.Inspection:
                        Color = Color.yellow;
                        break;
                    case StateBot.Detected:
                        //CustomDebug.Log($"_targetsTransforms.Count = {_targetsTransforms.Count}");
                        Color = Color.red;
                        break;
                    case StateBot.Died:
                        Color = Color.gray;
                        break;
                    default:
                        Color = Color.cyan;
                        break;
                }
            }
        }

            }
        }


        #region UnityMethods

        protected override void Awake()
        {
            base.Awake();
            Agent = GetComponent<NavMeshAgent>();
            _bodyBot = GetComponentInChildren<BodyBot>();
            _headBot = GetComponentInChildren<HeadBot>();
            _armBot = GetComponentInChildren<ArmBot>();
            _detectorBot = GetComponentInChildren<DetectorBot>();
            _flagBot = GetComponentInChildren<FlagBot>();

        }

        private void OnEnable()
        {
            if (_bodyBot != null) _bodyBot.OnApplyDamageChange += SetDamage;
            if (_headBot != null) _headBot.OnApplyDamageChange += SetDamage;
            if (_detectorBot != null) _detectorBot.OnTargetDetected += AddTargetToTargetsList;
            if (_detectorBot != null) _detectorBot.OnTargetLost += RemoveTargetFromTargetList;
            if (Weapon != null) Weapon.OnAmmoEnd += Weapon.ReloadClip;
        }

        private void OnDisable()
        {
            ClearEventSigning();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, _point);

            Gizmos.color = Color.cyan;
            var flat = new Vector3(1, 1, 1);
            Gizmos.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, flat);
            // Gizmos.DrawWireSphere(transform.position, Sence.ActiveDistance);
            Gizmos.DrawLine(transform.position, transform.position + Vector3.forward);

            //Gizmos.color = Color.yellow;
            //_visionPointBoarder.x = transform.forward.x + Vision.ActiveDis * Mathf.Cos(Mathf.Deg2Rad * (Vision.ActiveAng));
            //_visionPointBoarder.z = transform.forward.z + Vision.ActiveDis * Mathf.Sin(Mathf.Deg2Rad * (Vision.ActiveAng));
            //_visionPointBoarder.y = transform.forward.y;
            //Gizmos.DrawLine(transform.position, _visionPointBoarder);


            //_visionPointBoarder.x = transform.forward.x + Vision.ActiveDis * Mathf.Cos(Mathf.Deg2Rad * (-Vision.ActiveAng));
            //_visionPointBoarder.z = transform.forward.z + Vision.ActiveDis * Mathf.Sin(Mathf.Deg2Rad * (-Vision.ActiveAng));
            //_visionPointBoarder.y = transform.forward.y;
            //Gizmos.DrawLine(transform.position, _visionPointBoarder);

            //  Gizmos.color = Color.red;
            // Gizmos.DrawWireSphere(transform.position, Vision.ActiveDis);


        }

        #endregion


        #region Methods

        public void Tick()
        {
            switch (StateBot)
            {
                case StateBot.Died:
                    
                    ClearEventSigning();
                    
                    break;

                case StateBot.Detected:

                    RefreshStopingDistance();

                    if (ChooseTarget())
                    {
                        if (Vision.VisionM(transform, Target))
                        {
                            Weapon.Fire();
                            CaptureTarget(Target.position);

                            if (Agent.velocity == Vector3.zero)
                            {
                                AimingToTarget(Target.position);
                            }
                        }
                        else
                        {
                            if (Vision.FeelingTarget(transform, Target))
                            {
                                CaptureTarget(Target.position);
                            }
                            else
                            {
                                StateBot = StateBot.Patrol;
                            }
                        }

                    }
                    else
                    {
                        //CustomDebug.Log($"ChooseTarget false");
                        StateBot = StateBot.Patrol;
                    }


                    break;

                case StateBot.Inspection:

                    ObservingForEnemy();
                    if (!_isResetingState)
                    {
                        Invoke(nameof(ResetStateBot), _waitTime);
                        _isResetingState = true;
                    }

                    break;

                case StateBot.Patrol:

                    RefreshStopingDistance();
                    ObservingForEnemy();

                    //CustomDebug.Log($"Distance = {Vector3.Distance(_point, transform.position)}, stop - offset = {_stoppingDistanceForPatroling + _distanceOffset} ");
                    if (CustomVector.CheckDistanceMatch(_point, transform.position, _stoppingDistanceForPatroling) || Agent.velocity == Vector3.zero)
                    {
                        StateBot = StateBot.Inspection;
                        //CustomDebug.Log($"StateBot = {StateBot}");  
                    }
                    else
                    {
                        //CustomDebug.Log($"StateBot = {StateBot} after distance checked");
                        MoveToPoint(_point);
                    }

                    break;

                case StateBot.None:
                default:

                    InitializationPatroling();

                    break;

            }
        }

        private bool ChooseTarget()
        {
            var hasTarget = false;

            if (_targetsTransforms.Count > 0)
            {
                Target = _targetsTransforms[0];
                hasTarget = true;
            }
            else
            {
                Target = null;
            }

            return hasTarget;
        }

        private void AimingToTarget(Vector3 target)
        {
            Vector3 direction = target - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion lookAt = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
            transform.rotation = lookAt;
            _armBot.AimingToTarget(target);
        }

        private void CaptureTarget(Vector3 target)
        {
            _point = target;
            MoveToPoint(_point);
        }

        private void InitializationPatroling()
        {
            if (!Agent.hasPath || Agent.velocity == Vector3.zero)
            {
                _point = Patrol.GenericRandomPoint(transform);
                MoveToPoint(_point);
                Agent.stoppingDistance = _stoppingDistanceForPatroling;
            }
            StateBot = StateBot.Patrol;
        }

        private void ObservingForEnemy()
        {
            if (Target == null) return;
            if (Vision.VisionM(transform, Target))
            {
                StateBot = StateBot.Detected;
            }
        }

        private void AddTargetToTargetsList(ITargeted target)
        {
            if (target.GetAffiliation() == Affiliation.None) return;
            if (target.GetAffiliation() != _affiliationSide)
            {
                _targetsTransforms.Add(target.GetTransform());
                target.OnDeath += RemoveTargetFromTargetList;
                StateBot = StateBot.Detected;
            }
        }

        private void RemoveTargetFromTargetList(ITargeted target)
        {
            var targetTransform = target.GetTransform();
            RemoveTargetFromTargetList(targetTransform);
        }

        private void RemoveTargetFromTargetList(Transform target)
        {
            if (!_targetsTransforms.Contains(target))
            {
                return;
            }

            target.GetComponent<ITargeted>().OnDeath -= RemoveTargetFromTargetList;

            _targetsTransforms.Remove(target);

            if (_targetsTransforms.Count == 0)
            {
                StateBot = StateBot.Patrol;
            }
        }

        private void RefreshStopingDistance()
        {
            switch (StateBot)
            {
                case StateBot.Detected:
                    if (Agent.stoppingDistance != _stoppingDistanceForDetected)
                    {
                        Agent.stoppingDistance = _stoppingDistanceForDetected;
                    }
                    break;
                default:
                    if (Agent.stoppingDistance != _stoppingDistanceForPatroling)
                    {
                        Agent.stoppingDistance = _stoppingDistanceForPatroling;
                    }
                    break;

            }
        }

        private void ResetStateBot()
        {
            StateBot = StateBot.None;
            _isResetingState = false;
        }

        private void SetDamage(InfoCollision info)
        {
            //todo реакциия на попадание  
            if (Hp > 0)
            {
                Hp -= info.Damage;
                return;
            }

            if (Hp <= 0)
            {
                StateBot = StateBot.Died;
                Agent.enabled = false;
                foreach (var child in GetComponentsInChildren<Transform>())
                {
                    child.parent = null;

                    var tempRbChild = child.GetComponent<Rigidbody>();
                    if (!tempRbChild)
                    {
                        tempRbChild = child.gameObject.AddComponent<Rigidbody>();
                    }

                    tempRbChild.isKinematic = false;
                    tempRbChild.useGravity = true;
                    //tempRbChild.AddForce(info.Dir * Random.Range(10, 300));

                    Destroy(child.gameObject, 10);
                }

                AffiliationSide = Affiliation.None;
                OnDeath?.Invoke(this.transform);
                OnDieChange?.Invoke(this);
                //gameObject.GetComponent<Bot>().enabled = false;
            }
        }


        public void MoveToPoint(Vector3 point)

        {
            Agent.SetDestination(point);
        }

        private void SetSide()
        {
            _flagBot.AffiliationSide = _affiliationSide;
        }

        private void ClearEventSigning()
        {
            if (_bodyBot != null) _bodyBot.OnApplyDamageChange -= SetDamage;
            if (_headBot != null) _headBot.OnApplyDamageChange -= SetDamage;
            if (_detectorBot != null) _detectorBot.OnTargetDetected -= AddTargetToTargetsList;
            if (_detectorBot != null) _detectorBot.OnTargetLost -= RemoveTargetFromTargetList;
            if (Weapon != null) Weapon.OnAmmoEnd -= Weapon.ReloadClip;
        }

        #endregion


        #region ITargeted

        public Affiliation GetAffiliation()
        {
            if (StateBot == StateBot.Died)
            {
                return Affiliation.None;
            }
            else
            {
                return _affiliationSide;
            }

        }

        public Transform GetTransform()
        {
            return Transform;
        }

        #endregion

    }
}