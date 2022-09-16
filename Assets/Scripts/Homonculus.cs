using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Homonculus : MonoBehaviour
{
    public Part.DeathDelegate OnBrainDeath;

    private Part _brain;
    private Dictionary<Part, List<Part>> _connections;
    private CamFollowDouble camFollow;
    private bool isP1;
    private AudioClip[] lightHitSounds;
    private AudioClip[] bigHitSounds;
    private AudioClip[] stepSounds;
    private AudioSource audioPlayer;
    private float maxHitTimeCooldown = 0.3f;
    private float hitTimeCooldown;

    public void Start() {
        lightHitSounds = Resources.LoadAll<AudioClip>("light_hits");
        //lightBrainHits = Resources.LoadAll<AudioClip>("light_brain_hits");
        bigHitSounds = Resources.LoadAll<AudioClip>("big_hits");
        //bigBrainHits = Resources.LoadAll<AudioClip>("big_brain_hits");
        stepSounds = Resources.LoadAll<AudioClip>("metal_steps");

        //brainDeathSound = Resources.Load<AudioClip>("squish_1");
        //drillSound = Resources.Load<AudioClip>("drill_sound");
        //muscleStretch = Resources.Load<AudioClip>("stretching");
        //muscleBreak = Resources.Load<AudioClip>("rope_break");

        audioPlayer = gameObject.GetComponent<AudioSource>();

        audioPlayer.volume = PlayerPrefs.GetFloat("SFXVolume", 1);

        hitTimeCooldown = maxHitTimeCooldown;
    }

    public void BeginTracking(bool isP1)
    {
        _connections = new Dictionary<Part, List<Part>>();

        MyJoint[] allJoints = GetComponentsInChildren<MyJoint>();

        foreach (MyJoint j in allJoints)
        {
            if (j.connectedPart == null) {
                continue;
            }
            
            ReportConnection(j.transform.parent.GetComponent<Part>(), j.connectedPart);
        }

        this.isP1 = isP1;

        _brain = GetComponentInChildren<Brain>();
        
        //if (OnBrainDeath != null) {
            _brain.OnDeath += OnBrainDeath;
        //}
    }

    public void RecalculateNeurons()
    {

        Part[] allParts = GetComponentsInChildren<Part>();

        foreach (Part p in allParts)
        {
            p.visited = false;
            p.brainConnected = false;
        }

        RecursiveNeuronRecalculation(new List<Part>() { _brain });
    }

    private void RecursiveNeuronRecalculation(List<Part> incomingParts)
    {
        if (incomingParts.Count == 0)
        {
            return;
        }

        List<Part> outgoingParts = new List<Part>();

        foreach (Part p in incomingParts)
        {
            p.visited = true;
            p.brainConnected = true;

            List<Part> connectedParts;

            if (_connections.TryGetValue(p, out connectedParts))
            {
                foreach (Part p2 in connectedParts)
                {
                    if (!p2.visited)
                    {
                        outgoingParts.Add(p2);
                    }
                }
            }
        }

        RecursiveNeuronRecalculation(outgoingParts);
    }

    public void ReportBrain(Part brain)
    {
        _brain = brain;
    }

    public void ReportConnection(Part p1, Part p2)
    {
        if (_connections == null) {
            return;
        }

        if (_connections.ContainsKey(p1))
        {
            _connections[p1].Add(p2);
        }
        else
        {
            _connections.Add(p1, new List<Part> { p2 });
        }

        if (_connections.ContainsKey(p2))
        {
            _connections[p2].Add(p1);
        }
        else
        {
            _connections.Add(p2, new List<Part> { p1 });
        }
    }

    public void InvalidateConnection(Part p1, Part p2)
    {
        _connections[p1].Remove(p2);

        _connections[p2].Remove(p1);
    }

    public void ReportDestroyedPart(Part deadPart)
    {
        if (_connections == null) {
            return;
        }

        if (!_connections.ContainsKey(deadPart))
        {
            return;
        }

        foreach (Part p in _connections[deadPart])
        {
            _connections[p].Remove(deadPart);
        }

        _connections.Remove(deadPart);
    }

    public void ReportDestroyedPartAndRecalc(Part deadPart)
    {
        ReportDestroyedPart(deadPart);

        RecalculateNeurons();
    }

    public void StartGame()
    {
        BeginTracking(isP1);

        MyJoint[] allJoints = GetComponentsInChildren<MyJoint>();

        foreach (MyJoint j in allJoints)
        {
            if (j.connectedPart == null) {
                Destroy(j.gameObject);
                ReportDestroyedPart(j);
            }
        }

        camFollow = Camera.main.GetComponent<CamFollowDouble>();

        RecalculateNeurons();
    }

    public void Update() {
        if (hitTimeCooldown > 0) {
            hitTimeCooldown -= Time.deltaTime;
        }
    }

    public void PlayLightHit() {
        if (hitTimeCooldown <= 0) {
            hitTimeCooldown = maxHitTimeCooldown;

            audioPlayer.PlayOneShot(lightHitSounds.PickRandom());
        }
    }

    public void PlayBigHit() {
        if (hitTimeCooldown <= 0) {
            hitTimeCooldown = maxHitTimeCooldown;

            audioPlayer.PlayOneShot(bigHitSounds.PickRandom());
        }
    }

    public void PlayStep() {
        audioPlayer.PlayOneShot(stepSounds.PickRandom());
    }

    public void PlaySound(AudioClip sound) {
        audioPlayer.PlayOneShot(sound);
    }

    public void PlayCooldownSound(AudioClip sound) {
        if (hitTimeCooldown <= 0) {
            hitTimeCooldown = maxHitTimeCooldown;

            audioPlayer.PlayOneShot(sound);
        }
    }
}
