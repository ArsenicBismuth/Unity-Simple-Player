using UnityEngine;

public class Argument : MonoBehaviour
{

    VLCPlayer player;

    void Start()
    {
        player = gameObject.GetComponent<VLCPlayer>();

        string fileArg = GetArg("--file");
        if (!string.IsNullOrEmpty(fileArg))
        {
            Debug.Log("File argument: " + fileArg);
            string[] fileArgs = new string[] { fileArg };
            player.LoadVideo(fileArgs);
        }
        else
        {
            Debug.Log("No file argument provided.");
        }
    }

    private static string GetArg(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return null;
    }
}
