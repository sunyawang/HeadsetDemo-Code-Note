// Copyright 2014 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;
using System.Collections;

//添加碰撞器组件
[RequireComponent (typeof(Collider))]
//继承MonoBehaviour和IGvrGazeResponder
public class Teleport : MonoBehaviour, IGvrGazeResponder
{
	//开始位置
	private Vector3 startingPosition;

	//Start仅在Update函数第一次被调用前调用 只调用一次
	void Start ()
	{
		//transform代表了物体的位置、旋转和缩放，场景中每一个物体都有一个Transform
		startingPosition = transform.localPosition;
		//一开始设置render为红色
		SetGazedAt (false);
	}

	//当behavior被启用后，LateUpdate()函数在每一帧都被调用
	void LateUpdate ()
	{
		//GvrViewer.Instance获取GvrViewer的单例
		//UpdateState 获取最新的头部跟踪数据
		GvrViewer.Instance.UpdateState ();
		//当点击了VR返回键，应该和Android的系统返回键做一样的处理
		if (GvrViewer.Instance.BackButtonPressed) {
			//退出应用程序
			Application.Quit ();
		}
	}

	public void SetGazedAt (bool gazedAt)
	{
		//GetComponent<Type>()获取GameObject中type类型的组件，此处获取渲染器Renderer
		//render使物体显示在屏幕上，对于任何gameobjet或者compent，都可以通过render属性来访问到它的渲染器。如果想使物体不可见可以这么设置 renderer.enabled = false;
		//设置Render组件材质的颜色
		GetComponent<Renderer> ().material.color = gazedAt ? Color.green : Color.red;
		//test:设置render是否可见
//		Renderer render = GetComponent<Renderer>();
//		render.enabled = gazedAt;
	}

	//重置为默认值。Reset是在用户点击检视面板的Reset按钮或者首次添加该组件时被调用。
	public void Reset ()
	{
		//重置transform
		transform.localPosition = startingPosition;
	}

	public void ToggleVRMode ()
	{
		//VRModeEnabled决定是渲染VR场景还是普通场景
		GvrViewer.Instance.VRModeEnabled = !GvrViewer.Instance.VRModeEnabled;
	}

	public void ToggleDistortionCorrection ()
	{
		//决定使用哪个扭曲修正函数
		switch (GvrViewer.Instance.DistortionCorrection) {
		case GvrViewer.DistortionCorrectionMethod.Unity:
			GvrViewer.Instance.DistortionCorrection = GvrViewer.DistortionCorrectionMethod.Native;
			break;
		case GvrViewer.DistortionCorrectionMethod.Native:
			GvrViewer.Instance.DistortionCorrection = GvrViewer.DistortionCorrectionMethod.None;
			break;
		case GvrViewer.DistortionCorrectionMethod.None:
		default:
			GvrViewer.Instance.DistortionCorrection = GvrViewer.DistortionCorrectionMethod.Unity;
			break;
		}
	}

	public void ToggleDirectRender ()
	{
		//如果直接渲染在屏幕上置为true，如果先渲染到缓冲区设置为false。如果你想要延时渲染就把这个选项关掉。
		GvrViewer.Controller.directRender = !GvrViewer.Controller.directRender;
	}

	public void TeleportRandomly ()
	{
		//获取半径为1的球体在表面上的一个随机点
		Vector3 direction = Random.onUnitSphere;
		direction.y = Mathf.Clamp (direction.y, 0.5f, 1f);
		float distance = 2 * Random.value + 1.5f;
		transform.localPosition = direction * distance;
	}

	#region IGvrGazeResponder implementation

	/// Called when the user is looking on a GameObject with this script,
	/// as long as it is set to an appropriate layer (see GvrGaze).
	/// 当视线看向某个gameobject的时候回调
	public void OnGazeEnter ()
	{
		SetGazedAt (true);
	}

	/// Called when the user stops looking on the GameObject, after OnGazeEnter
	/// was already called.
	/// 实现离开gameobject时回调
	public void OnGazeExit ()
	{
		SetGazedAt (false);
	}

	/// Called when the viewer's trigger is used, between OnGazeEnter and OnGazeExit.
	/// 当trigger打开时，在视线看向gameobje并且为离开时候调用
	public void OnGazeTrigger ()
	{
		TeleportRandomly ();
	}

	#endregion
}
