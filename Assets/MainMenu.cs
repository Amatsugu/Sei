using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public GameObject menu;
	public GameObject help;

	public bool isHelp;

	// Start is called before the first frame update
	void Start()
	{
		help.SetActive(false);
	}

	// Update is called once per frame
	void Update()
	{
		if (!isHelp)
		{
			if(Input.GetKeyUp(KeyCode.Space))
				SceneManager.LoadScene(1);
		}
		if(Input.GetKeyUp(KeyCode.H))
		{
			isHelp = !isHelp;
			menu.SetActive(!isHelp);
			help.SetActive(isHelp);
		}
	}
}
