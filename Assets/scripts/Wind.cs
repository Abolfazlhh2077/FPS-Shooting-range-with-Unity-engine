
using UnityEngine;


public class wind : MonoBehaviour
{

    private Vector3 windVector;
    private Vector3 newWindVector;
    private float windChangeSpeed = 1f;
    private float Timer = 0;
    private float endTimer = 10f;
    private bool isTransitioning = false;


    void Start()
    {
        windVector = makeNewWind();
        newWindVector = windVector;
    }

    void Update()
    {

        
        if (!isTransitioning)
        {
            Timer += Time.deltaTime;
            if (Timer > endTimer)
            {
                Timer = 0;
                int dice = UnityEngine.Random.Range(0, 2);
                if (dice == 1)
                {
                    newWindVector = makeNewWind();
                }
            }

        }


        if (windVector != newWindVector)
        {
            isTransitioning = true;
            windVector = Vector3.MoveTowards(windVector, newWindVector, windChangeSpeed * Time.deltaTime);
        } else
        {
            isTransitioning = false;
        }

    }


    private Vector3 makeNewWind()
    {
        Vector2 circle = Random.insideUnitCircle.normalized * Random.Range(0, 6f);
        return new Vector3(circle.x, 0, circle.y);
    }

    public Vector3 getWind()
    {
        return windVector;
    }


}
