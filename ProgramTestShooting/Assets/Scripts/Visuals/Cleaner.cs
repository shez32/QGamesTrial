using UnityEngine;

public class BulletCleaner : MonoBehaviour
{
    [Header("Tags")] 
    
    [SerializeField] private string tagToClean;

    [SerializeField] private bool invertTag;
    
    private void OnTriggerExit(Collider other)
    {
        if (invertTag)
        {
            if(other.CompareTag("Powerup")) return;
            
            if (!other.CompareTag(tagToClean))
            {
                Destroy(other.gameObject);
            }
        }
        else
        {
            if (other.CompareTag(tagToClean))
            {
                Destroy(other.gameObject);
            }
        }
    }
}
