using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody rb;

    bool hasLanded = false;
    bool thrown = false;

    Vector3 initPosition;

    public int diceValue;
    public DiceSide[] diceSides;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initPosition = transform.position;
        rb.useGravity = false;

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            RollDice();
        }

        if(rb.IsSleeping() && !hasLanded && thrown)
        {
            hasLanded = true;
            rb.useGravity = false;
            SideValueCheck();
        }
        else if (rb.IsSleeping() && hasLanded && diceValue == 0)
        {
            RollAgain();
        }

    }
    void RollDice()
    {
        if(!thrown && !hasLanded)
        {
            thrown = true;
            rb.useGravity = true;
            rb.AddTorque(Random.Range(0, 500), Random.Range(0, 500), Random.Range(0, 500));
        }
        else if (thrown && hasLanded)
        {
            //Reset();
        }
    }
    public void RollAgain()
    {
        Reset();
        thrown = true;
        rb.useGravity = true;
        rb.AddTorque(Random.Range(0, 500), Random.Range(0, 500), Random.Range(0, 500));
    }

    public void Reset()
    {
        transform.position = initPosition;
        rb.useGravity = false;
        thrown = false;
        hasLanded = false;
    }

    void SideValueCheck()
    {
        diceValue = 0;
        foreach(var side in diceSides)
        {
            if(side.OnGround())
            {
                diceValue = side.sideValue;
            }
        }
    }

    public bool GetThrown() { return thrown; }
    public bool GetLanded() { return hasLanded; }

}
