using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VTuber {
	public class BgImage : MonoBehaviour {
		[SerializeField]
		GameObject rawImage;

		public void Activate() {
			rawImage.SetActive(true);
		}

		public void Deactivate() {
			rawImage.SetActive(false);
		}
	}
}
