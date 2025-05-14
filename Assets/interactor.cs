using System.Security.Cryptography;
using UnityEngine;



public class interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange;
    public PlayerMovementAdvanced pma;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray r = new Ray(InteractorSource.position, InteractorSource.forward);
            if (Physics.Raycast(r, out RaycastHit hitinfo, InteractRange))
            {
                if (hitinfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    interactObj.Interact(gameObject);
                }
            }
        }
    }
}
interface IInteractable
{
    public void Interact(GameObject player);
}
