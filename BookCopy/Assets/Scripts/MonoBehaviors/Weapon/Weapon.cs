using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Weapon : MonoBehaviour
{
    //Weapon System Field
    public GameObject ammoPrefab;
    static List<GameObject> ammoPool;
    public int poolSize;
    public float weaponVelocity;

    //Animation Field
    bool isFiring;

    [HideInInspector]
    public Animator animator;

    Camera localCamera;

    float positiveSlope;
    float negativeSlope;

    enum Quadrant
    {
        Up,
        Left,
        Right,
        Down
    }

    //////////
    //Weapon System
    //////////
    //Event
    private void Awake() 
    {
        if (ammoPool == null)
        {
            ammoPool = new List<GameObject>();
        }

        for (int i = 0; i < poolSize; i++)
        {
            GameObject ammoObject = Instantiate(ammoPrefab);
            ammoObject.SetActive(false);
            ammoPool.Add(ammoObject);   
         
        }
    }

    private void Start() {
        animator = GetComponent<Animator>();
        isFiring = false;
        localCamera = Camera.main;

        Vector2 lowerLeft  = localCamera.ScreenToWorldPoint(new Vector2(0           ,             0));
        Vector2 upperRight = localCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        Vector2 upperLeft  = localCamera.ScreenToWorldPoint(new Vector2(0           , Screen.height));
        Vector2 lowerRight = localCamera.ScreenToWorldPoint(new Vector2(Screen.width,             0));

        positiveSlope = Getslope(lowerLeft, upperRight);
        negativeSlope = Getslope(upperLeft, lowerRight);
    } 

    private void Update() {
        if(Input.GetMouseButtonDown(0))
        {
            isFiring = true;
            FireAmmo();
        }

        UpdateState();
    }

    private void OnDestroy() {
        ammoPool = null;
    }

    //Method
    GameObject SpawnAmmo(Vector3 location)
    {
        foreach(GameObject ammo in ammoPool)
        {
            if (ammo.activeSelf == false)
            {
                ammo.transform.position = location;
                ammo.SetActive(true);
                return ammo;
            }
        }
        return null;
    }

    void FireAmmo()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject ammo = SpawnAmmo(transform.position);

        if (ammo != null)
        {
            Arc arcScript = ammo.GetComponent<Arc>();
            float travelDuration = 1.0f / weaponVelocity;

            StartCoroutine(arcScript.TravelArc(mousePosition, travelDuration));
        }
    }

    float Getslope(Vector2 p1, Vector2 p2)
    {
        return (p2.y - p1.y) / (p2.x - p1.x);
    }

    bool HigherThanPositiveSlpoeLine(Vector2 inputPosition)
    {
        Vector2 playerPosition = gameObject.transform.position;
        Vector2 mousePosition = localCamera.ScreenToWorldPoint(inputPosition);
        
        float yIntercept = playerPosition.y - (positiveSlope * playerPosition.x);
        float inputIntercept = mousePosition.y - (positiveSlope * mousePosition.x);

        return inputIntercept > yIntercept;
    }

    bool HigherThanNegativeSlpoeLine(Vector2 inputPosition)
    {
        Vector2 playerPosition = gameObject.transform.position;
        Vector2 mousePosition = localCamera.ScreenToWorldPoint(inputPosition);
        
        float yIntercept = playerPosition.y - (negativeSlope * playerPosition.x);
        float inputIntercept = mousePosition.y - (negativeSlope * mousePosition.x);

        return inputIntercept > yIntercept;
    }

    Quadrant GetQuadrant()
    {
        bool higherThanPositiveSlpoeLine = HigherThanPositiveSlpoeLine(Input.mousePosition);
        bool higherThanNegativeSlpoeLine = HigherThanNegativeSlpoeLine(Input.mousePosition);

        if (!higherThanPositiveSlpoeLine && higherThanNegativeSlpoeLine)
        {
            return Quadrant.Right;
        }

        else if (!higherThanPositiveSlpoeLine && !higherThanNegativeSlpoeLine)
        {
            return Quadrant.Down;
        }

        else if (higherThanPositiveSlpoeLine && !higherThanNegativeSlpoeLine)
        {
            return Quadrant.Left;
        }

        else
        {
            return Quadrant.Up;
        }
    }

    void UpdateState()
    {
        if (isFiring)
        {
            Vector2 quadrantVector;
            Quadrant quadEnum = GetQuadrant();

            switch (quadEnum)
            {
                case Quadrant.Right:
                    quadrantVector = new Vector2(1.0f, 0.0f);
                    break;

                case Quadrant.Down:
                    quadrantVector = new Vector2(0.0f, -1.0f);
                    break;

                case Quadrant.Left:
                    quadrantVector = new Vector2(-1.0f, 0.0f);
                    break;

                case Quadrant.Up:
                    quadrantVector = new Vector2(0.0f, 1.0f);
                    break;

                default:
                    quadrantVector = new Vector2(0.0f, 0.0f);
                    break;
            }

            animator.SetBool("isFiring", true);

            animator.SetFloat("fireXDir", quadrantVector.x);
            animator.SetFloat("fireYDir", quadrantVector.y);

            isFiring = false;
        }
        else
        {
            animator.SetBool("isFiring", false);
        }
    }
}
