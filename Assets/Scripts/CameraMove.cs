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
		//Invoke("startMovement", 2.5f);
	}

	public void startMovement()
	{
		StartCoroutine(smoothMovement());
	}

	IEnumerator moveToWaypoint(GameObject wp)
	{
		WaitForEndOfFrame EOF = new WaitForEndOfFrame();
		
		float elapsedTime = 0f;
		Vector3 originalPosition = transform.position;
		for (int i = 0; i < waypoints.Length; i++)
		{
			moving = true;
			while (elapsedTime < TRANS_LERP_TIME)
			{ 
				elapsedTime += Time.deltaTime;
				transform.position = Vector3.Lerp(originalPosition, wp.transform.position, (elapsedTime / TRANS_LERP_TIME));
				yield return EOF;
				
			}
			moving = false;
		}
	   
	}

	
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
	/// move multiple objects towards one point over time
	/// </summary>
	/// <param name="objects">array of objects to move</param>
	/// <param name="wp">waypoint to move towards</param>
	/// <returns></returns>
	public IEnumerator moveObjectsToPoint(GameObject[] objects, GameObject wp)
	{
		WaitForEndOfFrame EOF = new WaitForEndOfFrame();

		float elapsedTime = 0f;
		Vector3[] originalPosition = new Vector3[objects.Length];
		for (int i = 0; i < objects.Length; i++)
		{
			originalPosition[i] = objects[i].transform.position;
		}

		moving = true;
		while (elapsedTime < LERP)
		{
			Vector3 VecLERP;
			elapsedTime += Time.deltaTime;
			for (int j = 0; j < objects.Length; j++)
			{
				if (objects[j].tag == "Car")
					VecLERP = Vector3.Lerp(originalPosition[j], wp.transform.position, (elapsedTime / (LERP+0.07f)));
				else
					VecLERP = Vector3.Lerp(originalPosition[j], wp.transform.position, (elapsedTime / LERP));
				

				VecLERP.z = originalPosition[j].z;
				VecLERP.y = originalPosition[j].y;

				//if (objects[j].tag == "Car")
				//    VecLERP.x -= 1f;

				objects[j].transform.position = VecLERP;
			}
				
			yield return EOF;

		}
		moving = false;
		
	}

	IEnumerator rotateAtWaypoint(GameObject wp)
	{
		WaitForEndOfFrame EOF = new WaitForEndOfFrame();
		float elapsedTime = 0f;
		Quaternion originalRot = transform.rotation;
		Quaternion rot = Quaternion.Euler(transform.rotation.x, wp.transform.rotation.y, transform.rotation.z);

		rotating = true;
		while (elapsedTime < ROT_LERP_TIME)
		{
			elapsedTime += Time.deltaTime;
			//transform.rotation = Quaternion.Lerp(originalRot, rot, (elapsedTime/ROT_LERP_TIME));
			transform.rotation = Quaternion.Slerp(originalRot, rot, (elapsedTime / ROT_LERP_TIME));
			yield return EOF;
		}
		rotating = false;
	}

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
