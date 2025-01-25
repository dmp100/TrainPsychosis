using CHARACTERS;
using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TESTING
{
    public class TestCharacters : MonoBehaviour
    {
        public TMP_FontAsset tempFont;

        // Start is called before the first frame update
        void Start()
        {
            //Character Elen = CharacterManager.instance.CreateCharacter("Elen");
            //Character Stella = CharacterManager.instance.CreateCharacter("Stella");
            //Character Stella2 = CharacterManager.instance.CreateCharacter("Stella");
            //Character Adam = CharacterManager.instance.CreateCharacter("Adam");
            StartCoroutine(Test());
        }

        IEnumerator Test()
        {

            Character Elen = CharacterManager.instance.CreateCharacter("Elen");
            Character Adam = CharacterManager.instance.CreateCharacter("Adam");
            Character Ben = CharacterManager.instance.CreateCharacter("Benjamin");

            List<string> lines = new List<string>()
        {
            "abcddf",
            "asfhlafjshkjaf.",
            "fygudsh",
            "gwes4gew {wa 1} sfafasfas"
        };
            yield return Elen.Say(lines);

            Elen.SetNameColor(Color.red);
            Elen.SetDialogueColor(Color.green);
            Elen.SetNameFont(tempFont);
            Elen.SetDialogueFont(tempFont);

            yield return Elen.Say(lines);

            Elen.ResetConfigurationData();

            yield return Elen.Say(lines);

            lines = new List<string>()
            {
                "fsafijasioj",
                "fsafasasffsa"
            };

            yield return Adam.Say(lines);

            yield return Ben.Say("fassf.{a} 저는 엄싸개죠.");

            Debug.Log("Finished");
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}