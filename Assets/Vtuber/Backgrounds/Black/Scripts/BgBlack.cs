﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VTuber {
	public class BgBlack : MonoBehaviour, Background {
		public void ChangeSkybox() {
			RenderSettings.skybox = null;
		}

		public Color GetBgColor() {
			return Color.black;
		}
	}
}