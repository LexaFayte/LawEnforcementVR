using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {

	public GameObject[] waypoints;
	private Coroutine co_translate;
	private Coroutine co_rotate;

	private bool moving;
	private bool rotating;

	private float TRANS_LERP_TIME = 5f; //time to move from one point to another
	private float LERP = 20f;
	private float ROT_LERP_TIME = 2f;
	private float SMOOTH_TIME = 3f;

	// Use this for initialization
	void Awake () {
		moving = false;
		rotating = false;
	}

    /// <summary>
    /// start movement for T1
    /// </summary>
	public void startMovement()
	{
		StartCoroutine(smoothMovement());
	}

	/// <summary>
    /// translates object over a given period of time
    /// </summary>
    /// <param name="obj">the object to translate</param>
    /// <param name="waypoint">waypoint of where to move</param>
    /// <param name="LerpTime">time to move</param>
    /// <returns></returns>
	public IEnumerator translateOverTime(GameObject obj, GameObject waypoint, float LerpTime)
	{
		WaitForEndOfFrame EOF = new WaitForEndOfFrame();
		float elapsedTime = 0f;
		Vector3 originalPosition;
		originalPosition = obj.transform.position;

		while (elapsedTime < LerpTime)
		{
			elapsedTime += Time.deltaTime;
			obj.transform.position = Vector3.Lerp(originalPosition, waypoint.transform.position, (elapsedTime / LerpTime));
			yield return EOF;
		}
	}

    /// <summary>
    /// moves camera over time in T1
    /// </summary>
    /// <returns></returns>
	IEnumerator smoothMovement()
	{
		WaitForEndOfFrame EOF = new WaitForEndOfFrame();
		float elapsedTime = 0f;
		Quaternion originalRot;
		Vector3 originalPosition;
		for (int i = 0; i < waypoints.Length; i++)
		{
			originalRot = transform.rotation;
			originalPosition = transform.position;

			while (elapsedTime < TRANS_LERP_TIME)
			{
				elapsedTime += Time.deltaTime;
				transform.position = Vector3.Lerp(originalPosition, waypoints[i].transform.position, (elapsedTime / TRANS_LERP_TIME));
				yield return EOF;
			}

			elapsedTime = 0f;
			if (waypoints[i].transform.eulerAngles.y != 0)
			{
				Quaternion rot = Quaternion.Euler(transform.rotation.x, 90f, transform.rotation.z);
				while (elapsedTime < ROT_LERP_TIME)
				{
					elapsedTime += Time.deltaTime;
					this.transform.rotation = Quaternion.Slerp(originalRot, rot, (elapsedTime / ROT_LERP_TIME));
					yield return EOF;
				}
			}
			elapsedTime = 0f;
		}

	}
}
