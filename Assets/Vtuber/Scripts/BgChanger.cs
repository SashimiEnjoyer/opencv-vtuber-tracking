using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VTuber {
	public class BgChanger : MonoBehaviour {
		//[SerializeField]
		//UnityEngine.UI.Text bgNameText;
		[SerializeField]
		BgImage bgImage;		
		[SerializeField]
		Camera cutCamera;
		[SerializeField]
		GameObject[] bgPrefabs;


		GameObject currentBg;
		int currentIndex;

		void Start() {
			currentIndex = 0;
			currentBg = null;
			ChangeTo(0);
		}

		void Update() {
			if (Input.GetKeyDown(KeyCode.Tab)) {
				if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
					currentIndex = (currentIndex > 0) ? currentIndex - 1 : bgPrefabs.Length - 1;
				} else {
					currentIndex = (currentIndex + 1 < bgPrefabs.Length) ? currentIndex + 1 : 0;
				}
				ChangeTo(currentIndex);
			}
		}

		void ChangeTo(int index) {
			if (index < bgPrefabs.Length) {
				if (currentBg != null) {
					Destroy(currentBg);
				}
				bgImage.Deactivate();
				currentBg = Instantiate<GameObject>(bgPrefabs[index]);
				Background bg = currentBg.GetComponent<Background>();
				bg.ChangeSkybox();
				Color bgColor = bg.GetBgColor();
				cutCamera.backgroundColor = bgColor;
				//bgNameText.text = bgPrefabs[index].name;
			}
		}
	}
}
