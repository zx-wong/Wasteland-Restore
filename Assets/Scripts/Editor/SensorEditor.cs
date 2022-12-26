using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Sensor))]
public class SensorEditor : Editor
{
    private void OnSceneGUI()
    {
        Vision();
        Hearing();
        Flee();
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private void Vision()
    {
        Sensor sensor = (Sensor)target;

        Handles.color = Color.black;
        Handles.DrawWireArc(sensor.transform.position, Vector3.up, Vector3.forward, 360, sensor.visionRadius);

        Vector3 viewAngle1 = DirectionFromAngle(sensor.transform.eulerAngles.y, -sensor.angle / 2);
        Vector3 viewAngle2 = DirectionFromAngle(sensor.transform.eulerAngles.y, sensor.angle / 2);

        Handles.color = Color.black;
        Handles.DrawLine(sensor.transform.position, sensor.transform.position + viewAngle1 * sensor.visionRadius);
        Handles.DrawLine(sensor.transform.position, sensor.transform.position + viewAngle2 * sensor.visionRadius);

        if (sensor.sawPlayer)
        {
            Handles.color = Color.green;
            Handles.DrawLine(sensor.transform.position, sensor.player.transform.position);
        }
    }

    private void Hearing()
    {
        Sensor sensor = (Sensor)target;

        Handles.color = Color.grey;
        Handles.DrawWireArc(sensor.transform.position, Vector3.up, Vector3.forward, 360, sensor.hearRadius);

        Vector3 viewAngle1 = DirectionFromAngle(sensor.transform.eulerAngles.y, -sensor.angle / 2);
        Vector3 viewAngle2 = DirectionFromAngle(sensor.transform.eulerAngles.y, sensor.angle / 2);
    }

    private void Flee()
    {
        Sensor sensor = (Sensor)target;

        Handles.color = Color.yellow;
        Handles.DrawWireArc(sensor.transform.position, Vector3.up, Vector3.forward, 360, sensor.fleeRadius);

        Vector3 viewAngle1 = DirectionFromAngle(sensor.transform.eulerAngles.y, -sensor.angle / 2);
        Vector3 viewAngle2 = DirectionFromAngle(sensor.transform.eulerAngles.y, sensor.angle / 2);
    }
}
