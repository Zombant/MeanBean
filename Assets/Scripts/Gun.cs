using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Ammo Type", menuName = "Ammo Type")]
public class Gun : ScriptableObject {

    [Header("Name must match the tag of the corresponding prefab")]
    public string name;

    [Space]

    [Header("Values used in every gun")]
    public int damage;
    public float range;

    //public int numberOfShots;
    public int ammoUsage;

    public Mesh mesh;

    //public float recoil;
    public float timeToNextShot;

    [Header("Shotgun Specific")]
    public int minNumOfPellets;
    public int maxNumOfPellets;
}
