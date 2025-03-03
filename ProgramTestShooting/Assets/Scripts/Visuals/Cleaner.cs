using UnityEngine;

public class BulletCleaner : MonoBehaviour
{
    [Header("Tags")] 
    [SerializeField] private string tagToClean; //i.e the tag that we want to destroy
    [SerializeField] private bool invertTag; //when set to true the tag provided above will not be destroyed and everything else will be
    
    private void OnTriggerExit(Collider other)
    {
        if (invertTag)
        {
            //if it is a powerup, don't do anything......we wont destroy any powerups due to running coroutines
            if(other.CompareTag("Powerup")) return;
            
            //if the object is other than the specified tag than destroy it
            if (!other.CompareTag(tagToClean))
            {
                Destroy(other.gameObject);
            }
        }
        else
        {
            //if it is the specified tag than destroy it
            if (other.CompareTag(tagToClean))
            {
                Destroy(other.gameObject);
            }
        }
    }
}
