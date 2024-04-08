using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace ToJ
{
	[ExecuteInEditMode]
	[AddComponentMenu("Alpha Mask")]
	[RequireComponent(typeof(MeshRenderer))]
	public class Mask : MonoBehaviour
	{
		[SerializeField]
		private bool _isMaskingEnabled = true;

		[SerializeField]
		private bool _clampAlphaHorizontally;

		[SerializeField]
		private bool _clampAlphaVertically;

		[SerializeField]
		private float _clampingBorder = 0.01f;

		[SerializeField]
		private bool _useMaskAlphaChannel;

		[SerializeField]
		private Texture mainTex;

		[SerializeField]
		private Vector2 mainTexTiling = new Vector2(1f, 1f);

		[SerializeField]
		private Vector2 mainTexOffset = new Vector2(0f, 0f);

		private bool fullMaskRefresh = true;

		private Matrix4x4 oldWorldToMask = Matrix4x4.identity;

		private Shader maskedSpriteWorldCoordsShader;

		private Shader maskedUnlitWorldCoordsShader;

		private Material spritesAlphaMaskWorldCoords;

		private const string SPRITES_RESOURCE_ADDRESS = "Materials/Sprites-Alpha-Mask-WorldCoords";

		public const string MASKED_SPRITE_SHADER = "Alpha Masked/Sprites Alpha Masked - World Coords";

		public const string MASKED_UNLIT_SHADER = "Alpha Masked/Unlit Alpha Masked - World Coords";

		private Material maskMaterial;

		private const string MASK_RESOURCE_ADDRESS = "Materials/Mask-Material";

		private Matrix4x4 maskQuadMatrix = Matrix4x4.identity;

		private MaterialPropertyBlock maskeePropertyBlock;

		private MaterialPropertyBlock maskPropertyBlock;

		public List<Material> createdMatsStorage = new List<Material>();

		[SerializeField]
		private int instanceID;

		public bool IsMaskingEnabled
		{
			get
			{
				return _isMaskingEnabled;
			}
			set
			{
				if (value != _isMaskingEnabled)
				{
					fullMaskRefresh = true;
					_isMaskingEnabled = value;
				}
			}
		}

		public bool ClampAlphaHorizontally
		{
			get
			{
				return _clampAlphaHorizontally;
			}
			set
			{
				if (value != _clampAlphaHorizontally)
				{
					fullMaskRefresh = true;
					_clampAlphaHorizontally = value;
				}
			}
		}

		public bool ClampAlphaVertically
		{
			get
			{
				return _clampAlphaVertically;
			}
			set
			{
				if (value != _clampAlphaVertically)
				{
					fullMaskRefresh = true;
					_clampAlphaVertically = value;
				}
			}
		}

		public float ClampingBorder
		{
			get
			{
				return _clampingBorder;
			}
			set
			{
				if (value != _clampingBorder)
				{
					fullMaskRefresh = true;
					_clampingBorder = value;
				}
			}
		}

		public bool UseMaskAlphaChannel
		{
			get
			{
				return _useMaskAlphaChannel;
			}
			set
			{
				if (value != _useMaskAlphaChannel)
				{
					fullMaskRefresh = true;
					_useMaskAlphaChannel = value;
				}
			}
		}

		public Texture MainTex
		{
			get
			{
				return mainTex;
			}
			set
			{
				if (value != mainTex)
				{
					fullMaskRefresh = true;
					mainTex = value;
				}
			}
		}

		public Vector2 MainTexTiling
		{
			get
			{
				return mainTexTiling;
			}
			set
			{
				if (value != mainTexTiling)
				{
					fullMaskRefresh = true;
					mainTexTiling = value;
				}
			}
		}

		public Vector2 MainTexOffset
		{
			get
			{
				return mainTexOffset;
			}
			set
			{
				if (value != mainTexOffset)
				{
					fullMaskRefresh = true;
					mainTexOffset = value;
				}
			}
		}

		private Shader MaskedSpriteWorldCoordsShader
		{
			get
			{
				if (maskedSpriteWorldCoordsShader == null)
				{
					maskedSpriteWorldCoordsShader = Shader.Find("Alpha Masked/Sprites Alpha Masked - World Coords");
				}
				return maskedSpriteWorldCoordsShader;
			}
			set
			{
				maskedSpriteWorldCoordsShader = value;
			}
		}

		private Shader MaskedUnlitWorldCoordsShader
		{
			get
			{
				if (maskedUnlitWorldCoordsShader == null)
				{
					maskedUnlitWorldCoordsShader = Shader.Find("Alpha Masked/Unlit Alpha Masked - World Coords");
				}
				return maskedUnlitWorldCoordsShader;
			}
			set
			{
				maskedUnlitWorldCoordsShader = value;
			}
		}

		public Material SpritesAlphaMaskWorldCoords
		{
			get
			{
				if (spritesAlphaMaskWorldCoords == null)
				{
					spritesAlphaMaskWorldCoords = Resources.Load<Material>("Materials/Sprites-Alpha-Mask-WorldCoords");
					if (spritesAlphaMaskWorldCoords == null)
					{
						UnityEngine.Debug.LogError("Materials/Sprites-Alpha-Mask-WorldCoords not found!");
					}
				}
				return spritesAlphaMaskWorldCoords;
			}
			set
			{
				spritesAlphaMaskWorldCoords = value;
			}
		}

		public Material MaskMaterial
		{
			get
			{
				if (maskMaterial == null)
				{
					maskMaterial = Resources.Load<Material>("Materials/Mask-Material");
					if (maskMaterial == null)
					{
						UnityEngine.Debug.LogError("Materials/Mask-Material not found!");
					}
				}
				return maskMaterial;
			}
			set
			{
				maskMaterial = value;
			}
		}

		public MaterialPropertyBlock MaskeePropertyBlock
		{
			get
			{
				if (maskeePropertyBlock == null)
				{
					maskeePropertyBlock = new MaterialPropertyBlock();
				}
				return maskeePropertyBlock;
			}
			set
			{
				maskeePropertyBlock = value;
			}
		}

		public MaterialPropertyBlock MaskPropertyBlock
		{
			get
			{
				if (maskPropertyBlock == null)
				{
					maskPropertyBlock = new MaterialPropertyBlock();
				}
				return maskPropertyBlock;
			}
			set
			{
				maskPropertyBlock = value;
			}
		}

		public void FlagForRefresh()
		{
			fullMaskRefresh = true;
		}

		private void Start()
		{
			MeshRenderer component = GetComponent<MeshRenderer>();
			if (Application.isPlaying && component != null)
			{
				component.enabled = false;
			}
		}

		private void ClearShaders()
		{
			MaskedSpriteWorldCoordsShader = null;
			MaskedUnlitWorldCoordsShader = null;
		}

		private void InitializeMeshRenderer(MeshRenderer target)
		{
			target.shadowCastingMode = ShadowCastingMode.Off;
			target.receiveShadows = false;
		}

		private void LateUpdate()
		{
			RefreshMaskPropertyBlock();
			UpdateMasking();
		}

		private void UpdateInstanciatedMaterials(List<Material> differentMaterials, Matrix4x4 worldToMask)
		{
			foreach (Material differentMaterial in differentMaterials)
			{
				ValidateShader(differentMaterial);
				if (differentMaterial.shader == MaskedSpriteWorldCoordsShader || differentMaterial.shader == MaskedUnlitWorldCoordsShader)
				{
					differentMaterial.DisableKeyword("_SCREEN_SPACE_UI");
					differentMaterial.SetTexture("_AlphaTex", MainTex);
					differentMaterial.SetTextureOffset("_AlphaTex", MainTexOffset);
					differentMaterial.SetTextureScale("_AlphaTex", MainTexTiling);
					differentMaterial.SetFloat("_ClampHoriz", ClampAlphaHorizontally ? 1 : 0);
					differentMaterial.SetFloat("_ClampVert", ClampAlphaVertically ? 1 : 0);
					differentMaterial.SetFloat("_ClampBorder", ClampingBorder);
					differentMaterial.SetMatrix("_WorldToMask", worldToMask);
				}
			}
		}

		private void UpdateUIMaterials(List<Graphic> differentGraphics, Matrix4x4 worldToMask, Vector4 tilingAndOffset)
		{
			foreach (Graphic differentGraphic in differentGraphics)
			{
				if (differentGraphic.material.shader == MaskedSpriteWorldCoordsShader || differentGraphic.material.shader == MaskedUnlitWorldCoordsShader)
				{
					UIMaterialModifier uIMaterialModifier = differentGraphic.GetComponent<UIMaterialModifier>();
					if (uIMaterialModifier == null)
					{
						uIMaterialModifier = differentGraphic.gameObject.AddComponent<UIMaterialModifier>();
					}
					Canvas canvas = differentGraphic.canvas;
					Matrix4x4 rhs = Matrix4x4.identity;
					bool flag = canvas != null && (canvas.renderMode == RenderMode.ScreenSpaceOverlay || (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null));
					bool screenSpaceEnabled = false;
					if (flag)
					{
						RectTransform component = canvas.GetComponent<RectTransform>();
						rhs = Matrix4x4.TRS(component.rect.size / 2f * canvas.scaleFactor, Quaternion.identity, Vector3.one * canvas.scaleFactor);
						screenSpaceEnabled = true;
					}
					uIMaterialModifier.SetMaskToMaskee(worldToMask * rhs, tilingAndOffset, ClampingBorder, IsMaskingEnabled, screenSpaceEnabled, ClampAlphaHorizontally, ClampAlphaVertically, UseMaskAlphaChannel, differentGraphic is Text);
					uIMaterialModifier.UpdateAlphaTex(MainTex);
					uIMaterialModifier.ApplyMaterialProperties();
				}
			}
		}

		private void UpdateSpriteMaterials(List<SpriteRenderer> differentSpriteRenderers, Matrix4x4 worldToMask, Vector4 tilingAndOffset)
		{
			foreach (SpriteRenderer differentSpriteRenderer in differentSpriteRenderers)
			{
				if (differentSpriteRenderer.sharedMaterial.shader == MaskedSpriteWorldCoordsShader || differentSpriteRenderer.sharedMaterial.shader == MaskedUnlitWorldCoordsShader)
				{
					differentSpriteRenderer.GetPropertyBlock(MaskeePropertyBlock);
					Texture texture = MaskeePropertyBlock.GetTexture("_AlphaTex");
					if (MainTex != null)
					{
						MaskeePropertyBlock.SetTexture("_AlphaTex", MainTex);
					}
					MaskeePropertyBlock.SetFloat("_ClampHoriz", ClampAlphaHorizontally ? 1 : 0);
					MaskeePropertyBlock.SetFloat("_ClampVert", ClampAlphaVertically ? 1 : 0);
					MaskeePropertyBlock.SetFloat("_UseAlphaChannel", UseMaskAlphaChannel ? 1 : 0);
					MaskeePropertyBlock.SetFloat("_Enabled", IsMaskingEnabled ? 1 : 0);
					MaskeePropertyBlock.SetFloat("_ClampingBorder", ClampingBorder);
					MaskeePropertyBlock.SetFloat("_IsThisText", 0f);
					MaskeePropertyBlock.SetVector("_AlphaTex_ST", tilingAndOffset);
					MaskeePropertyBlock.SetMatrix("_WorldToMask", worldToMask);
					differentSpriteRenderer.SetPropertyBlock(MaskeePropertyBlock);
				}
			}
		}

		public void UpdateMasking()
		{
			if (MaskedSpriteWorldCoordsShader == null || MaskedUnlitWorldCoordsShader == null)
			{
				UnityEngine.Debug.Log("Shaders necessary for masking don't seem to be present in the project.");
			}
			else
			{
				if (!(base.transform.parent != null))
				{
					return;
				}
				Vector2 vector = MainTexTiling;
				float x = vector.x;
				Vector2 vector2 = MainTexTiling;
				float y = vector2.y;
				Vector2 vector3 = MainTexOffset;
				float x2 = vector3.x;
				Vector2 vector4 = MainTexOffset;
				Vector4 tilingAndOffset = new Vector4(x, y, x2, vector4.y);
				RectTransform component = GetComponent<RectTransform>();
				Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
				Vector3 vector5 = Vector3.one;
				if ((bool)component)
				{
					vector5 = component.rect.size;
					vector5.z = 1f;
				}
				vector5 = Vector3.Scale(vector5, base.transform.lossyScale);
				worldToLocalMatrix.SetTRS(base.transform.position, base.transform.rotation, vector5);
				worldToLocalMatrix = Matrix4x4.Inverse(worldToLocalMatrix);
				if (worldToLocalMatrix != oldWorldToMask)
				{
					fullMaskRefresh = true;
				}
				oldWorldToMask = worldToLocalMatrix;
				if (!fullMaskRefresh)
				{
					return;
				}
				Renderer[] componentsInChildren = base.transform.parent.gameObject.GetComponentsInChildren<Renderer>(includeInactive: true);
				Graphic[] componentsInChildren2 = base.transform.parent.gameObject.GetComponentsInChildren<Graphic>(includeInactive: true);
				List<SpriteRenderer> list = new List<SpriteRenderer>();
				List<Graphic> list2 = new List<Graphic>();
				List<Material> list3 = new List<Material>();
				Renderer[] array = componentsInChildren;
				foreach (Renderer renderer in array)
				{
					if (renderer is SpriteRenderer)
					{
						if (renderer.gameObject != base.gameObject && !list.Contains((SpriteRenderer)renderer))
						{
							list.Add((SpriteRenderer)renderer);
						}
						continue;
					}
					Material[] sharedMaterials = renderer.sharedMaterials;
					foreach (Material material in sharedMaterials)
					{
						if (material != null && !list3.Contains(material))
						{
							list3.Add(material);
						}
					}
				}
				Graphic[] array2 = componentsInChildren2;
				foreach (Graphic graphic in array2)
				{
					if (graphic.gameObject != base.gameObject && !list2.Contains(graphic))
					{
						list2.Add(graphic);
					}
				}
				UpdateInstanciatedMaterials(list3, worldToLocalMatrix);
				UpdateUIMaterials(list2, worldToLocalMatrix, tilingAndOffset);
				UpdateSpriteMaterials(list, worldToLocalMatrix, tilingAndOffset);
				fullMaskRefresh = false;
			}
		}

		private void ValidateShader(Material material)
		{
			if (material.shader.ToString() == MaskedSpriteWorldCoordsShader.ToString() && material.shader.GetInstanceID() != MaskedSpriteWorldCoordsShader.GetInstanceID())
			{
				UnityEngine.Debug.Log("There seems to be more than one masked shader in the project with the same display name, and it's preventing the mask from being properly applied.");
				MaskedSpriteWorldCoordsShader = null;
			}
			if (material.shader.ToString() == MaskedUnlitWorldCoordsShader.ToString() && material.shader.GetInstanceID() != MaskedUnlitWorldCoordsShader.GetInstanceID())
			{
				UnityEngine.Debug.Log("There seems to be more than one masked shader in the project with the same display name, and it's preventing the mask from being properly applied.");
				MaskedUnlitWorldCoordsShader = null;
			}
		}

		private void RefreshMaskPropertyBlock()
		{
			if (MaskPropertyBlock == null)
			{
				MaskPropertyBlock = new MaterialPropertyBlock();
			}
			GetComponent<Renderer>().GetPropertyBlock(MaskPropertyBlock);
			if (MainTex != null)
			{
				MaskPropertyBlock.SetTexture("_MainTex", MainTex);
			}
			MaterialPropertyBlock materialPropertyBlock = MaskPropertyBlock;
			Vector2 vector = MainTexTiling;
			float x = vector.x;
			Vector2 vector2 = MainTexTiling;
			float y = vector2.y;
			Vector2 vector3 = MainTexOffset;
			float x2 = vector3.x;
			Vector2 vector4 = MainTexOffset;
			materialPropertyBlock.SetVector("_MainTex_ST", new Vector4(x, y, x2, vector4.y));
			GetComponent<Renderer>().SetPropertyBlock(MaskPropertyBlock);
		}

		private void GetMaskQuad(Mesh mesh, Rect r)
		{
			Vector3[] array = new Vector3[4]
			{
				new Vector3(r.xMin, r.yMin, 0f),
				new Vector3(r.xMax, r.yMin, 0f),
				new Vector3(r.xMin, r.yMax, 0f),
				new Vector3(r.xMax, r.yMax, 0f)
			};
			int[] array2 = new int[6]
			{
				0,
				2,
				1,
				2,
				3,
				1
			};
			Vector3[] array3 = new Vector3[4]
			{
				-Vector3.forward,
				-Vector3.forward,
				-Vector3.forward,
				-Vector3.forward
			};
			Vector2[] array4 = new Vector2[4]
			{
				new Vector2(0f, 0f),
				new Vector2(1f, 0f),
				new Vector2(0f, 1f),
				new Vector2(1f, 1f)
			};
			if (!BasicArrayCompare(mesh.vertices, array))
			{
				mesh.vertices = array;
			}
			if (!BasicArrayCompare(mesh.triangles, array2))
			{
				mesh.triangles = array2;
			}
			if (!BasicArrayCompare(mesh.normals, array3))
			{
				mesh.normals = array3;
			}
			if (!BasicArrayCompare(mesh.uv, array4))
			{
				mesh.uv = array4;
			}
		}

		private bool BasicArrayCompare<T>(T[] one, T[] two)
		{
			if (one.Length != two.Length)
			{
				return false;
			}
			for (int i = 0; i < one.Length; i++)
			{
				if (!one[i].Equals(two[i]))
				{
					return false;
				}
			}
			return true;
		}

		private List<Material> CollectDifferentMaterials()
		{
			List<Material> list = new List<Material>();
			if (base.transform.parent == null)
			{
				return list;
			}
			Renderer[] componentsInChildren = base.transform.parent.gameObject.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				if (!(renderer.gameObject != base.gameObject))
				{
					continue;
				}
				Material[] sharedMaterials = renderer.sharedMaterials;
				foreach (Material item in sharedMaterials)
				{
					if (!list.Contains(item))
					{
						list.Add(item);
						UnityEngine.Debug.Log(renderer.gameObject.GetComponent<Renderer>().gameObject);
					}
				}
			}
			return list;
		}

		public void ChangeMaskTexture(Texture texture)
		{
			MainTex = texture;
		}

		public void SetMaskRendererActive(bool value)
		{
			if (GetComponent<Renderer>() != null)
			{
				if (value)
				{
					GetComponent<Renderer>().enabled = true;
				}
				else
				{
					GetComponent<Renderer>().enabled = false;
				}
			}
		}

		public void DuplicateMaskedMaterials()
		{
			List<Material> list = CollectDifferentMaterials();
			Dictionary<Material, Material> dictionary = new Dictionary<Material, Material>();
			if (list.Count == 0)
			{
				return;
			}
			UnityEngine.Debug.Log("Different Materials: " + list.Count);
			foreach (Material item in list)
			{
				if (item.shader == MaskedSpriteWorldCoordsShader || item.shader == MaskedUnlitWorldCoordsShader)
				{
					Material value = new Material(item);
					dictionary.Add(item, value);
				}
			}
			UnityEngine.Debug.Log("Duplicate different Materials: " + dictionary.Count);
			foreach (Material value2 in dictionary.Values)
			{
				UnityEngine.Debug.Log("Material ID: " + value2.GetInstanceID(), value2);
			}
			if (base.transform.parent == null)
			{
				UnityEngine.Debug.Log("Proper mask hierarchy not found.");
				return;
			}
			Renderer[] componentsInChildren = base.transform.parent.gameObject.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				if (!(renderer.gameObject != base.gameObject))
				{
					continue;
				}
				Material[] array2 = new Material[renderer.sharedMaterials.Length];
				for (int j = 0; j < array2.Length; j++)
				{
					if (dictionary.ContainsKey(renderer.sharedMaterials[j]))
					{
						array2[j] = dictionary[renderer.sharedMaterials[j]];
					}
					else
					{
						array2[j] = renderer.sharedMaterials[j];
					}
				}
				renderer.sharedMaterials = array2;
			}
			Graphic[] componentsInChildren2 = base.transform.parent.gameObject.GetComponentsInChildren<Graphic>();
			Graphic[] array3 = componentsInChildren2;
			foreach (Graphic graphic in array3)
			{
				if (graphic.gameObject != base.gameObject && dictionary.ContainsKey(graphic.material))
				{
					graphic.material = dictionary[graphic.material];
				}
			}
		}
	}
}
