using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Giblet : MonoBehaviour
{
    public Sprite[] possibleSprites;
    public float velDeviation;
    public float torqueDeviation;
    public float resetSeconds;
    public AudioClip[] hitSounds;

    private AudioSource player;

    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = possibleSprites.PickRandom<Sprite>();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(Random.Range(-velDeviation, velDeviation), Random.Range(-velDeviation, velDeviation)));
        rb.AddTorque(Random.Range(-torqueDeviation, torqueDeviation));

        player = GetComponent<AudioSource>();

        StartCoroutine(ResetAfterSeconds());
    }

    IEnumerator ResetAfterSeconds()
    {
        yield return new WaitForSeconds(resetSeconds);

        Debug.Log("running scene reset");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Destroy(gameObject);
    }

    public void OnCollisionEnter2D(Collision2D col)
    {

        if (col.gameObject.tag == "Giblet")
        {
            return;
        }

        if (!player.isPlaying)
        {
            player.clip = hitSounds.PickRandom<AudioClip>();
            player.Play();
        }
    }
}
