using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AmplifyColor
{
	// Token: 0x02000316 RID: 790
	[Serializable]
	public class VolumeEffect
	{
		// Token: 0x060010CD RID: 4301 RVA: 0x000509BA File Offset: 0x0004EBBA
		public VolumeEffect(AmplifyColorBase effect)
		{
			this.gameObject = effect;
			this.components = new List<VolumeEffectComponent>();
		}

		// Token: 0x060010CE RID: 4302 RVA: 0x000509D4 File Offset: 0x0004EBD4
		public static VolumeEffect BlendValuesToVolumeEffect(VolumeEffectFlags flags, VolumeEffect volume1, VolumeEffect volume2, float blend)
		{
			VolumeEffect volumeEffect = new VolumeEffect(volume1.gameObject);
			foreach (VolumeEffectComponentFlags volumeEffectComponentFlags in flags.components)
			{
				if (volumeEffectComponentFlags.blendFlag)
				{
					VolumeEffectComponent volumeEffectComponent = volume1.FindEffectComponent(volumeEffectComponentFlags.componentName);
					VolumeEffectComponent volumeEffectComponent2 = volume2.FindEffectComponent(volumeEffectComponentFlags.componentName);
					if (volumeEffectComponent != null && volumeEffectComponent2 != null)
					{
						VolumeEffectComponent volumeEffectComponent3 = new VolumeEffectComponent(volumeEffectComponent.componentName);
						foreach (VolumeEffectFieldFlags volumeEffectFieldFlags in volumeEffectComponentFlags.componentFields)
						{
							if (volumeEffectFieldFlags.blendFlag)
							{
								VolumeEffectField volumeEffectField = volumeEffectComponent.FindEffectField(volumeEffectFieldFlags.fieldName);
								VolumeEffectField volumeEffectField2 = volumeEffectComponent2.FindEffectField(volumeEffectFieldFlags.fieldName);
								if (volumeEffectField != null && volumeEffectField2 != null)
								{
									VolumeEffectField volumeEffectField3 = new VolumeEffectField(volumeEffectField.fieldName, volumeEffectField.fieldType);
									string fieldType = volumeEffectField3.fieldType;
									if (!(fieldType == "System.Single"))
									{
										if (!(fieldType == "System.Boolean"))
										{
											if (!(fieldType == "UnityEngine.Vector2"))
											{
												if (!(fieldType == "UnityEngine.Vector3"))
												{
													if (!(fieldType == "UnityEngine.Vector4"))
													{
														if (fieldType == "UnityEngine.Color")
														{
															volumeEffectField3.valueColor = Color.Lerp(volumeEffectField.valueColor, volumeEffectField2.valueColor, blend);
														}
													}
													else
													{
														volumeEffectField3.valueVector4 = Vector4.Lerp(volumeEffectField.valueVector4, volumeEffectField2.valueVector4, blend);
													}
												}
												else
												{
													volumeEffectField3.valueVector3 = Vector3.Lerp(volumeEffectField.valueVector3, volumeEffectField2.valueVector3, blend);
												}
											}
											else
											{
												volumeEffectField3.valueVector2 = Vector2.Lerp(volumeEffectField.valueVector2, volumeEffectField2.valueVector2, blend);
											}
										}
										else
										{
											volumeEffectField3.valueBoolean = volumeEffectField2.valueBoolean;
										}
									}
									else
									{
										volumeEffectField3.valueSingle = Mathf.Lerp(volumeEffectField.valueSingle, volumeEffectField2.valueSingle, blend);
									}
									volumeEffectComponent3.fields.Add(volumeEffectField3);
								}
							}
						}
						volumeEffect.components.Add(volumeEffectComponent3);
					}
				}
			}
			return volumeEffect;
		}

		// Token: 0x060010CF RID: 4303 RVA: 0x00050C44 File Offset: 0x0004EE44
		public VolumeEffectComponent AddComponent(Component c, VolumeEffectComponentFlags compFlags)
		{
			if (compFlags == null)
			{
				VolumeEffectComponent volumeEffectComponent = new VolumeEffectComponent(string.Concat(c.GetType()));
				this.components.Add(volumeEffectComponent);
				return volumeEffectComponent;
			}
			VolumeEffectComponent volumeEffectComponent2;
			if ((volumeEffectComponent2 = this.FindEffectComponent(string.Concat(c.GetType()))) != null)
			{
				volumeEffectComponent2.UpdateComponent(c, compFlags);
				return volumeEffectComponent2;
			}
			VolumeEffectComponent volumeEffectComponent3 = new VolumeEffectComponent(c, compFlags);
			this.components.Add(volumeEffectComponent3);
			return volumeEffectComponent3;
		}

		// Token: 0x060010D0 RID: 4304 RVA: 0x00050CA7 File Offset: 0x0004EEA7
		public void RemoveEffectComponent(VolumeEffectComponent comp)
		{
			this.components.Remove(comp);
		}

		// Token: 0x060010D1 RID: 4305 RVA: 0x00050CB8 File Offset: 0x0004EEB8
		public void UpdateVolume()
		{
			if (this.gameObject == null)
			{
				return;
			}
			foreach (VolumeEffectComponentFlags volumeEffectComponentFlags in this.gameObject.EffectFlags.components)
			{
				if (volumeEffectComponentFlags.blendFlag)
				{
					Component component = this.gameObject.GetComponent(volumeEffectComponentFlags.componentName);
					if (component != null)
					{
						this.AddComponent(component, volumeEffectComponentFlags);
					}
				}
			}
		}

		// Token: 0x060010D2 RID: 4306 RVA: 0x00050D4C File Offset: 0x0004EF4C
		public void SetValues(AmplifyColorBase targetColor)
		{
			VolumeEffectFlags effectFlags = targetColor.EffectFlags;
			GameObject gameObject = targetColor.gameObject;
			foreach (VolumeEffectComponentFlags volumeEffectComponentFlags in effectFlags.components)
			{
				if (volumeEffectComponentFlags.blendFlag)
				{
					Component component = gameObject.GetComponent(volumeEffectComponentFlags.componentName);
					VolumeEffectComponent volumeEffectComponent = this.FindEffectComponent(volumeEffectComponentFlags.componentName);
					if (!(component == null) && volumeEffectComponent != null)
					{
						foreach (VolumeEffectFieldFlags volumeEffectFieldFlags in volumeEffectComponentFlags.componentFields)
						{
							if (volumeEffectFieldFlags.blendFlag)
							{
								FieldInfo field = component.GetType().GetField(volumeEffectFieldFlags.fieldName);
								VolumeEffectField volumeEffectField = volumeEffectComponent.FindEffectField(volumeEffectFieldFlags.fieldName);
								if (!(field == null) && volumeEffectField != null)
								{
									string fullName = field.FieldType.FullName;
									if (!(fullName == "System.Single"))
									{
										if (!(fullName == "System.Boolean"))
										{
											if (!(fullName == "UnityEngine.Vector2"))
											{
												if (!(fullName == "UnityEngine.Vector3"))
												{
													if (!(fullName == "UnityEngine.Vector4"))
													{
														if (fullName == "UnityEngine.Color")
														{
															field.SetValue(component, volumeEffectField.valueColor);
														}
													}
													else
													{
														field.SetValue(component, volumeEffectField.valueVector4);
													}
												}
												else
												{
													field.SetValue(component, volumeEffectField.valueVector3);
												}
											}
											else
											{
												field.SetValue(component, volumeEffectField.valueVector2);
											}
										}
										else
										{
											field.SetValue(component, volumeEffectField.valueBoolean);
										}
									}
									else
									{
										field.SetValue(component, volumeEffectField.valueSingle);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060010D3 RID: 4307 RVA: 0x00050F6C File Offset: 0x0004F16C
		public void BlendValues(AmplifyColorBase targetColor, VolumeEffect other, float blendAmount)
		{
			VolumeEffectFlags effectFlags = targetColor.EffectFlags;
			GameObject gameObject = targetColor.gameObject;
			for (int i = 0; i < effectFlags.components.Count; i++)
			{
				VolumeEffectComponentFlags volumeEffectComponentFlags = effectFlags.components[i];
				if (volumeEffectComponentFlags.blendFlag)
				{
					Component component = gameObject.GetComponent(volumeEffectComponentFlags.componentName);
					VolumeEffectComponent volumeEffectComponent = this.FindEffectComponent(volumeEffectComponentFlags.componentName);
					VolumeEffectComponent volumeEffectComponent2 = other.FindEffectComponent(volumeEffectComponentFlags.componentName);
					if (!(component == null) && volumeEffectComponent != null && volumeEffectComponent2 != null)
					{
						for (int j = 0; j < volumeEffectComponentFlags.componentFields.Count; j++)
						{
							VolumeEffectFieldFlags volumeEffectFieldFlags = volumeEffectComponentFlags.componentFields[j];
							if (volumeEffectFieldFlags.blendFlag)
							{
								FieldInfo field = component.GetType().GetField(volumeEffectFieldFlags.fieldName);
								VolumeEffectField volumeEffectField = volumeEffectComponent.FindEffectField(volumeEffectFieldFlags.fieldName);
								VolumeEffectField volumeEffectField2 = volumeEffectComponent2.FindEffectField(volumeEffectFieldFlags.fieldName);
								if (!(field == null) && volumeEffectField != null && volumeEffectField2 != null)
								{
									string fullName = field.FieldType.FullName;
									if (!(fullName == "System.Single"))
									{
										if (!(fullName == "System.Boolean"))
										{
											if (!(fullName == "UnityEngine.Vector2"))
											{
												if (!(fullName == "UnityEngine.Vector3"))
												{
													if (!(fullName == "UnityEngine.Vector4"))
													{
														if (fullName == "UnityEngine.Color")
														{
															field.SetValue(component, Color.Lerp(volumeEffectField.valueColor, volumeEffectField2.valueColor, blendAmount));
														}
													}
													else
													{
														field.SetValue(component, Vector4.Lerp(volumeEffectField.valueVector4, volumeEffectField2.valueVector4, blendAmount));
													}
												}
												else
												{
													field.SetValue(component, Vector3.Lerp(volumeEffectField.valueVector3, volumeEffectField2.valueVector3, blendAmount));
												}
											}
											else
											{
												field.SetValue(component, Vector2.Lerp(volumeEffectField.valueVector2, volumeEffectField2.valueVector2, blendAmount));
											}
										}
										else
										{
											field.SetValue(component, volumeEffectField2.valueBoolean);
										}
									}
									else
									{
										field.SetValue(component, Mathf.Lerp(volumeEffectField.valueSingle, volumeEffectField2.valueSingle, blendAmount));
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060010D4 RID: 4308 RVA: 0x000511CC File Offset: 0x0004F3CC
		public VolumeEffectComponent FindEffectComponent(string compName)
		{
			for (int i = 0; i < this.components.Count; i++)
			{
				if (this.components[i].componentName == compName)
				{
					return this.components[i];
				}
			}
			return null;
		}

		// Token: 0x060010D5 RID: 4309 RVA: 0x00051218 File Offset: 0x0004F418
		public static Component[] ListAcceptableComponents(AmplifyColorBase go)
		{
			if (go == null)
			{
				return new Component[0];
			}
			return Enumerable.ToArray<Component>(Enumerable.Where<Component>(go.GetComponents(typeof(Component)), (Component comp) => comp != null && !string.Concat(comp.GetType()).StartsWith("UnityEngine.") && !(comp.GetType() == typeof(AmplifyColorBase))));
		}

		// Token: 0x060010D6 RID: 4310 RVA: 0x0005126E File Offset: 0x0004F46E
		public string[] GetComponentNames()
		{
			return Enumerable.ToArray<string>(Enumerable.Select<VolumeEffectComponent, string>(this.components, (VolumeEffectComponent r) => r.componentName));
		}

		// Token: 0x04000FC9 RID: 4041
		public AmplifyColorBase gameObject;

		// Token: 0x04000FCA RID: 4042
		public List<VolumeEffectComponent> components;
	}
}
