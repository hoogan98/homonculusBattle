using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinPhraseGenerator : MonoBehaviour
{
    public Text output;

    private string[] estWords = new string[]{
        "Fastest",
        "Best",
        "Bestest",
        "Greatest",
        "Brightest",
        "Averagest",
        "Lightest",
        "Quickest",
        "Roundest",
        "Luckiest",
        "Dumbest",
        "Worst",
        "Second-Best",
        "Sexiest",
        "Cutest"
    };

    private string[] midWords = new string[]{
        "Synchronized",
        "Competitive",
        "Long-Distance",
        "Top-Speed",
        "Top-Thrill",
        "Downhill",
        "Cross-Country",
        "Water",
        "Aerial"
    };

    private string[] erWords = new string[]{
        "Swimmer",
        "Diver",
        "Runner",
        "Baller",
        "Pilot",
        "Fighter",
        "Boxer",
        "Champion",
        "Powerlifter",
        "Rollerblader",
        "Skateboarder",
        "BMX Pro",
        "Gamer",
        "Murderer",
        "Loser"
    };

    private string[] punctuation = new string[]{
        "!",
        "?",
        "*",
        ".",
        "",
        ",",
        "™",
        "℠",
        "®",
        "©"
    };

    public void Start() {
        if (output == null) {
            return;
        }

        output.text = GetTitle();
    }

    public string GetTitle()
    {
        return estWords.PickRandom<string>() + " " 
            + midWords.PickRandom<string>() + " " 
            + erWords.PickRandom<string>() + punctuation.PickRandom<string>();
    }
}
