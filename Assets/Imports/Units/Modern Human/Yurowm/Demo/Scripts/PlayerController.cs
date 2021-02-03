using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Animator))]
public class PlayerController : MonoBehaviour {

	public Transform rightGunBone;
	public Transform leftGunBone;
	public Arsenal[] arsenal;

	private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (arsenal.Length > 0)
            SetArsenal(arsenal[Random.Range(1, 3)].name);
        if (Random.Range(0f, 1f) > 0.5f) animator.SetBool("Squat", true);
    }

    [ContextMenu("Setup AK")]
    void SetupAK()
    {
        SetArsenal(arsenal[2].name);
    }

    [ContextMenu("Setup Sniper")]
    void SetupSniper()
    {
        SetArsenal(arsenal[1].name);
    }

    [ContextMenu("Turn Into Soldier")]
    void TurnIntoSoldier()
    {
        gameObject.AddComponent<Unit>();
        DestroyImmediate(GetComponent<Actions>());
        DestroyImmediate(this);
    }

	public void SetArsenal(string name) {
        animator = GetComponent<Animator>();
		foreach (Arsenal hand in arsenal) {
			if (hand.name == name) {
				if (rightGunBone.childCount > 0)
                {
                    if (Application.isEditor) DestroyImmediate(rightGunBone.GetChild(0).gameObject);
                    else Destroy(rightGunBone.GetChild(0).gameObject);
                }
				if (leftGunBone.childCount > 0)
                {
                    if (Application.isEditor) DestroyImmediate(leftGunBone.GetChild(0).gameObject);
                    else Destroy(leftGunBone.GetChild(0).gameObject);
                }
                if (hand.rightGun != null) {
					GameObject newRightGun = Instantiate(hand.rightGun);
					newRightGun.transform.parent = rightGunBone;
					newRightGun.transform.localPosition = Vector3.zero;
					newRightGun.transform.localRotation = Quaternion.Euler(90, 0, 0);
					}
				if (hand.leftGun != null) {
					GameObject newLeftGun = Instantiate(hand.leftGun);
					newLeftGun.transform.parent = leftGunBone;
					newLeftGun.transform.localPosition = Vector3.zero;
					newLeftGun.transform.localRotation = Quaternion.Euler(90, 0, 0);
				}
				animator.runtimeAnimatorController = hand.controller;
				return;
				}
		}
	}

	[System.Serializable]
	public struct Arsenal {
		public string name;
		public GameObject rightGun;
		public GameObject leftGun;
		public RuntimeAnimatorController controller;
	}
}
