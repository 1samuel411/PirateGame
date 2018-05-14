using PirateGame.UI.Views;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace PirateGame.UI.Controllers
{
    public class EditCharacterController : Controller
    {

        public EditCharacterView editCharacterView;

        public BodyComponent bodyComponent;

        public BodyComponent.MainType mainType
        {
            get
            {
                return bodyComponent.mainType;
            }
        }

        [ShowIf("mainType", BodyComponent.MainType.Selectable)]
        public int selected;
        [ShowIf("mainType", BodyComponent.MainType.Slider)]
        public float value;

        public string description;

        public delegate void Changed(EditCharacterController controller);
        public Changed changedDelegate;

        private void Awake()
        {
            initialized = true;
            UpdateUI();
        }

        private void Start()
        {
            initialized = false;
        }

        private void Update()
        {
            UpdateUI();
        }

        private bool initialized;
        void UpdateUI()
        {
            editCharacterView.seperatorGameObject.SetActive(false);
            editCharacterView.sliderGameObject.SetActive(false);
            editCharacterView.selectGameObject.SetActive(false);
            
            // Seperator
            if (bodyComponent == null)
            {
                editCharacterView.seperatorGameObject.SetActive(true);
                editCharacterView.seperatorText.text = description;
                return;
            }
            // Selectable
            if (bodyComponent.mainType == BodyComponent.MainType.Selectable)
            {
                editCharacterView.selectGameObject.SetActive(true);
                editCharacterView.selectTextName.text = bodyComponent.description;
                if (bodyComponent.type == BodyComponent.Type.color)
                {
                    editCharacterView.selectTextDesc.text = "Color " + (selected + 1);
                }
                else if (bodyComponent.type == BodyComponent.Type.gender)
                {
                    editCharacterView.selectTextDesc.text = selected == 0 ? "Male" : "Female";
                }
                else if (bodyComponent.type == BodyComponent.Type.wardrobeRecipe)
                {
                    editCharacterView.selectTextDesc.text = bodyComponent.bodyParts[selected].name;
                }

                if (!initialized)
                {
                    initialized = true;
                    changedDelegate(this);
                }
            }
            // Slider
            if (bodyComponent.mainType == BodyComponent.MainType.Slider)
            {
                editCharacterView.sliderGameObject.SetActive(true);
                editCharacterView.sliderNameText.text = bodyComponent.description;

                if(!initialized)
                {
                    initialized = true;
                    editCharacterView.slider.value = value;
                    changedDelegate(this);
                }
            }
        }

        public void SetValue(float value)
        {
            this.value = value;
            changedDelegate.Invoke(this);
        }

        public void SelectLeft()
        {
            if(selected <= 0)
            {
                // Reset
                if (bodyComponent.type == BodyComponent.Type.color)
                {
                    selected = bodyComponent.colors.Length - 1;
                }
                else if (bodyComponent.type == BodyComponent.Type.gender)
                {
                    selected = 1;
                }
                else if (bodyComponent.type == BodyComponent.Type.wardrobeRecipe)
                {
                    selected = bodyComponent.bodyParts.Length - 1;
                }
            }
            else
            {
                selected--;
            }
            changedDelegate.Invoke(this);
        }

        public void SelectRight()
        {
            int max = 0;
            // Reset
            if (bodyComponent.type == BodyComponent.Type.color)
            {
                max = bodyComponent.colors.Length - 1;
            }
            else if (bodyComponent.type == BodyComponent.Type.gender)
            {
                max = 1;
            }
            else if (bodyComponent.type == BodyComponent.Type.wardrobeRecipe)
            {
                max = bodyComponent.bodyParts.Length - 1;
            }
            if (selected >= max)
            {
                selected = 0;
            }
            else
            {
                selected++;
            }
            changedDelegate.Invoke(this);
        }

    }
}