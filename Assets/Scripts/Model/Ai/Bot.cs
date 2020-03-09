using System;
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
        public Transform Target { get; set; }
        public NavMeshAgent Agent { get; private set; }
        private float _waitTime = 3;
        private StateBot _stateBot;
        private Vector3 _point;
        private float _stoppingDistance = 2.0f;

        public event Action<Bot> OnDieChange;

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

        protected override void Awake()
        {
            base.Awake();
            Agent = GetComponent<NavMeshAgent>();
        }

        private void OnEnable()
        {
            var bodyBot = GetComponentInChildren<BodyBot>();
            if (bodyBot != null) bodyBot.OnApplyDamageChange += SetDamage;

            var headBot = GetComponentInChildren<HeadBot>();
            if (headBot != null) headBot.OnApplyDamageChange += SetDamage;
        }

        private void OnDisable()
        {
            var bodyBot = GetComponentInChildren<BodyBot>();
            if (bodyBot != null) bodyBot.OnApplyDamageChange -= SetDamage;

            var headBot = GetComponentInChildren<HeadBot>();
            if (headBot != null) headBot.OnApplyDamageChange -= SetDamage;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, Sence.ActiveDistance);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, Vision.ActiveDis);

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, _point);
        }

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
                    }
                    else
                    {
                        if (Sence.FeelingTarget(transform, Target))
                        {
                            _point = Target.position;
                            MoveToPoint(_point);
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
                    if (Vector3.Distance(_point, transform.position) <= 1)
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
    }
}