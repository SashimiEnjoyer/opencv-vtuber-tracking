using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VTuber {
	public interface Background {
		void ChangeSkybox();
		Color GetBgColor();
	}
}
