using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class EventCollisionFilteredTrigger : MonoBehaviour
{
    [SerializeField] private LayerMask m_layersDetected;

    [Header("Events")] [SerializeField] private UnityEvent<GameObject> m_onTriggerFilteredEnter;

    [SerializeField] private UnityEvent<GameObject> m_onTriggerFilteredStay;

    [SerializeField] private UnityEvent<GameObject> m_onTriggerFilteredExit;

    public UnityEvent<GameObject> triggerFilteredEnter => m_onTriggerFilteredEnter;
    public UnityEvent<GameObject> triggerFilteredStay => m_onTriggerFilteredStay;
    public UnityEvent<GameObject> triggerFilteredExit => m_onTriggerFilteredExit;

    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_layersDetected.HaveLayer(other.gameObject.layer)) m_onTriggerFilteredEnter.Invoke(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_layersDetected.HaveLayer(other.gameObject.layer)) m_onTriggerFilteredExit.Invoke(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (m_layersDetected.HaveLayer(other.gameObject.layer)) m_onTriggerFilteredStay.Invoke(other.gameObject);
    }
}