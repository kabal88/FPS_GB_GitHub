using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Geekbrains
{
    public sealed class Bot : BaseObjectScene
    {
        public float Hp = 100;
        public Vision Vision;
        public Sence Sence;
        public Weapon Weapon; //todo с разным оружием

        [SerializeField] private float _stoppingDistance = 2.0f;
        [SerializeField] private Affiliation _affiliationSide;

        private float _waitTime = 3;
        private StateBot _stateBot;
        private BodyBot _bodyBot;
        private HeadBot _headBot;
        private Vector3 _point;
        private Vector3 _visionPointBoarder;

        public event Action<Bot> OnDieChange;

        public Transform Target { get; set; }
        public List<Transform> Targets { get; set; }
        public NavMeshAgent Agent { get; private set; }
        public Affiliation AffiliationSide
        {
            get => _affiliationSide;
            set
            {
                _affiliationSide = value;
                SetSide();
            }
        }


        private StateBot StateBot
        {
            get => _stateBot;
            set
            {
                _stateBot = value;
                switch (value)
                {
                    case StateBot.None:
                        Color = Color.white;
                        break;
                    case StateBot.Patrol:
                        Color = Color.green;
                        break;
                    case StateBot.Inspection:
                        Color = Color.yellow;
                        break;
                    case StateBot.Detected:
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


        #region UnityMethods

        protected override void Awake()
        {
            base.Awake();
            Agent = GetComponent<NavMeshAgent>();
            _bodyBot = GetComponentInChildren<BodyBot>();
            _headBot = GetComponentInChildren<HeadBot>();
        }

        private void OnEnable()
        {
            if (_bodyBot != null) _bodyBot.OnApplyDamageChange += SetDamage;
            if (_headBot != null) _headBot.OnApplyDamageChange += SetDamage;
        }

        private void OnDisable()
        {
            if (_bodyBot != null) _bodyBot.OnApplyDamageChange -= SetDamage;
            if (_headBot != null) _headBot.OnApplyDamageChange -= SetDamage;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, Sence.ActiveDistance);
            Gizmos.DrawLine(transform.position, transform.position + Vector3.forward * Vision.ActiveDis);

            Gizmos.color = Color.yellow;
            _visionPointBoarder.x = transform.position.x + Vision.ActiveDis * Mathf.Cos(Mathf.Deg2Rad * (Vision.ActiveAng + 90));
            _visionPointBoarder.z = transform.position.z + Vision.ActiveDis * Mathf.Sin(Mathf.Deg2Rad * (Vision.ActiveAng + 90));
            _visionPointBoarder.y = transform.position.y;
            Gizmos.DrawLine(transform.position, _visionPointBoarder);


            _visionPointBoarder.x = transform.position.x + Vision.ActiveDis * Mathf.Cos(Mathf.Deg2Rad * (-Vision.ActiveAng + 90));
            _visionPointBoarder.z = transform.position.z + Vision.ActiveDis * Mathf.Sin(Mathf.Deg2Rad * (-Vision.ActiveAng + 90));
            _visionPointBoarder.y = transform.position.y;
            Gizmos.DrawLine(transform.position, _visionPointBoarder);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, Vision.ActiveDis);

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, _point);
        }

        #endregion

        public void Tick()
        {
            switch (StateBot)
            {
                case StateBot.Died:
                    break;

                case StateBot.Detected:

                    RefreshStopingDistance();
                    if (Vision.VisionM(transform, Target))
                    {
                        Weapon.Fire();
                        CaptureTarget(Target.position);
                    }
                    else
                    {
                        if (Sence.FeelingTarget(transform, Target))
                        {
                            CaptureTarget(Target.position);
                        }
                        else
                        {
                            StateBot = StateBot.Patrol;
                        }
                    }
                    break;

                case StateBot.Inspection:

                    ObservingForEnemy();
                    break;

                case StateBot.Patrol:

                    RefreshStopingDistance();
                    ObservingForEnemy();
                    if (Vector3.Distance(_point, transform.position) <= _stoppingDistance)
                    {
                        StateBot = StateBot.Inspection;
                        Invoke(nameof(ResetStateBot), _waitTime);
                    }
                    else
                    {
                        MoveToPoint(_point);
                    }
                    break;

                case StateBot.None:
                default:

                    InitializationPatroling();
                    break;

            }
        }

        private void CaptureTarget(Vector3 target)
        {
            _point = target;
            MoveToPoint(_point);
        }

        private void InitializationPatroling()
        {
            if (!Agent.hasPath)
            {
                _point = Patrol.GenericPoint(transform);
                MoveToPoint(_point);
                Agent.stoppingDistance = 0;
            }
            StateBot = StateBot.Patrol;
        }

        private void ObservingForEnemy()
        {
            if (Vision.VisionM(transform, Target))
            {
                StateBot = StateBot.Detected;
            }
        }

        private void RefreshStopingDistance()
        {
            switch (StateBot)
            {
                case StateBot.Detected:
                    if (Agent.stoppingDistance != _stoppingDistance)
                    {
                        Agent.stoppingDistance = _stoppingDistance;
                    }
                    break;
                default:
                    if (Agent.stoppingDistance != 0.0f)
                    {
                        Agent.stoppingDistance = 0;
                    }
                    break;

            }
        }

        private void ResetStateBot()
        {
            StateBot = StateBot.None;
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
                    //tempRbChild.AddForce(info.Dir * Random.Range(10, 300));
                    
                    Destroy(child.gameObject, 10);
                }

                OnDieChange?.Invoke(this);
            }
        }

        public void MoveToPoint(Vector3 point)
        {
            Agent.SetDestination(point);
        }

        private void SetSide()
        {
            var flag = GetComponentInChildren<FlagBot>();
            if (flag != null)
            {
                flag.AffiliationSide = _affiliationSide;
            }
        }
    }
}