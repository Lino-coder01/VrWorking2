using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script draws a rope between the edge of the fishing rod and the cylinder,
//IT uses a Bezier curve to make the rope look slightly curved instead of perfectly straight

//CODE TAKEN FROM
public class RopeControllerSimple : MonoBehaviour
{
    public Transform whatTheRopeIsConnectedTo;
    public Transform whatIsHangingFromTheRope;

    private LineRenderer lineRenderer;

    //A list with all rope sections
    public List<Vector3> allRopeSections = new List<Vector3>();

    //Rope data
    public float ropeLength = 4f; // augmente pour descendre plus bas
    private float minRopeLength = 1f;
    private float maxRopeLength = 4f;

    //Mass of what the rope is carrying
    private float loadMass = 1;
    //How fast we can add more/less rope
    float winchSpeed = 2f;

    //The joint we use to approximate the rope tension and length changes
    SpringJoint springJoint;

    //The script finds the springjoint on anchor obj, gets line renderer, sets handing object mass to a loadMass
    void Start()
    {
        springJoint = whatTheRopeIsConnectedTo.GetComponent<SpringJoint>();

        lineRenderer = GetComponent<LineRenderer>();

        whatIsHangingFromTheRope.GetComponent<Rigidbody>().mass = loadMass;
    }

    void Update()
    {
        //Add more/less rope
        UpdateWinch();

        //Display the rope with a line renderer
        DisplayRope();
    }

    //Update the spring constant and the length of the spring
    private void UpdateSpring()
    {
        float kRope = 50f;

        springJoint.spring = kRope;
        springJoint.damper = kRope * 2f; // damper > spring = pas de rebond
        springJoint.maxDistance = ropeLength;
    }

    //calculates four Bezier control points
    private void DisplayRope()
    {
        //This is not the actual width, but the width use so we can see the rope
        float ropeWidth = 0.002f;

        lineRenderer.startWidth = ropeWidth;
        lineRenderer.endWidth = ropeWidth;


        //Update the list with rope sections by approximating the rope with a bezier curve
        //A Bezier curve needs 4 control points
        Vector3 A = whatTheRopeIsConnectedTo.position; //topAnchor pos
        Vector3 D = whatIsHangingFromTheRope.position; //handing obj pos

        //Upper control point
        //To get a little curve at the top than at the bottom
        Vector3 B = A + whatTheRopeIsConnectedTo.up * (-(A - D).magnitude * 0.1f);
        //B = A;

        //Lower control point
        Vector3 C = D + whatIsHangingFromTheRope.up * ((A - D).magnitude * 0.5f);

        //Get the positions
        BezierCurve.GetBezierCurve(A, B, C, D, allRopeSections);


        //An array with all rope section positions
        Vector3[] positions = new Vector3[allRopeSections.Count];

        for (int i = 0; i < allRopeSections.Count; i++)
        {
            positions[i] = allRopeSections[i];
        }

        //Add the positions to the line renderer
        lineRenderer.positionCount = positions.Length;

        lineRenderer.SetPositions(positions);
    }

    //Add more/less rope
    private void UpdateWinch()
    {
        bool hasChangedRope = false;

        //More rope
        if (Input.GetKey(KeyCode.O) && ropeLength < maxRopeLength)
        {
            ropeLength += winchSpeed * Time.deltaTime;

            hasChangedRope = true;
        }
        else if (Input.GetKey(KeyCode.I) && ropeLength > minRopeLength)
        {
            ropeLength -= winchSpeed * Time.deltaTime;

            hasChangedRope = true;
        }


        if (hasChangedRope)
        {
            ropeLength = Mathf.Clamp(ropeLength, minRopeLength, maxRopeLength);

            //Need to recalculate the k-value because it depends on the length of the rope
            UpdateSpring();
        }
    }
}

//Approximate the rope with a bezier curve
public static class BezierCurve
{
    //Update the positions of the rope section
    public static void GetBezierCurve(Vector3 A, Vector3 B, Vector3 C, Vector3 D, List<Vector3> allRopeSections)
    {
        //The resolution of the line
        //Make sure the resolution is adding up to 1, so 0.3 will give a gap at the end, but 0.2 will work
        float resolution = 0.1f;

        //Clear the list
        allRopeSections.Clear();


        float t = 0;

        while (t <= 1f)
        {
            //Find the coordinates between the control points with a Bezier curve
            Vector3 newPos = DeCasteljausAlgorithm(A, B, C, D, t);

            allRopeSections.Add(newPos);

            //Which t position are we at?
            t += resolution;
        }

        allRopeSections.Add(D);
    }

    //The De Casteljau's Algorithm
    static Vector3 DeCasteljausAlgorithm(Vector3 A, Vector3 B, Vector3 C, Vector3 D, float t)
    {
        //Linear interpolation = lerp = (1 - t) * A + t * B
        //Could use Vector3.Lerp(A, B, t)

        //To make it faster
        float oneMinusT = 1f - t;

        //Layer 1
        Vector3 Q = oneMinusT * A + t * B;
        Vector3 R = oneMinusT * B + t * C;
        Vector3 S = oneMinusT * C + t * D;

        //Layer 2
        Vector3 P = oneMinusT * Q + t * R;
        Vector3 T = oneMinusT * R + t * S;

        //Final interpolated position
        Vector3 U = oneMinusT * P + t * T;

        return U;
    }
}