using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Brain.Graph.Nodes
{
    /// <summary>
    /// Компоненты нод
    /// </summary>
    public abstract class AINodeComponent : AINode
    {
        protected SerializedObject _serializedObject; 
        
        protected AINodeComponent()
        {
        }

        /// <summary>
        /// Настройка нода
        /// </summary>
        /// <param name="brain">Мозги</param>
        /// <param name="component">Компонент</param>
        /// <typeparam name="T">Тип компоненты</typeparam>
        public abstract void Setup(AIBrain brain, Type component);
        /// <summary>
        /// Настройка по созданному компоненту
        /// </summary>
        /// <param name="brain"></param>
        /// <param name="component"></param>
        public abstract void Setup(AIBrain brain, Component component);
        
        protected void Initialize(Component obj)
        {
            _serializedObject = new SerializedObject(obj);

            BindAllProperties(_serializedObject, this);

            RegisterCallback<ChangeEvent<UnityEngine.Object>>(_ => ApplyChanges(obj));
            RegisterCallback<ChangeEvent<bool>>(_ => ApplyChanges(obj));
            RegisterCallback<ChangeEvent<int>>(_ => ApplyChanges(obj));
            RegisterCallback<ChangeEvent<float>>(_ => ApplyChanges(obj));
            RegisterCallback<ChangeEvent<string>>(_ => ApplyChanges(obj));
            RegisterCallback<ChangeEvent<Vector2>>(_ => ApplyChanges(obj));
            RegisterCallback<ChangeEvent<Vector3>>(_ => ApplyChanges(obj));
            RegisterCallback<ChangeEvent<Color>>(_ => ApplyChanges(obj));
            RegisterCallback<ChangeEvent<AnimationCurve>>(_ => ApplyChanges(obj));
        }

        /// <summary>
        /// Рекурсивно создаёт PropertyField для всех видимых свойств
        /// </summary>
        private void BindAllProperties(SerializedObject so, VisualElement root)
        {
            var iterator = so.GetIterator();
            if (iterator.NextVisible(true)) // вход в объект
            {
                // Пропускаем "m_Script" и другие служебные поля
                do
                {
                    if (iterator.name == "m_Script" ||
                        iterator.name == "Label") 
                        continue;
                    
                    if (iterator.isArray && iterator.propertyType != SerializedPropertyType.String)
                    {
                        // Массивы/списки: создаём заголовок + дочерние элементы
                        var foldout = new Foldout { text = iterator.displayName, value = true };
                        var arrayRoot = new VisualElement();
                        
                        var arrayIterator = iterator.Copy();
                        int endIndex = arrayIterator.arraySize;
                        for (int i = 0; i < endIndex; i++)
                        {
                            var elementProp = iterator.GetArrayElementAtIndex(i);
                            var field = new PropertyField(elementProp);
                            field.Bind(so); // Важно: привязка к SerializedObject
                            arrayRoot.Add(field);
                        }
                        
                        foldout.Add(arrayRoot);
                        root.Add(foldout);
                    }
                    else
                    {
                        // Обычное поле
                        var field = new PropertyField(iterator.Copy());
                        field.Bind(so);
                        root.Add(field);
                    }
                }
                while (iterator.NextVisible(false)); // false = не заходить в детей повторно
            }
        }

        private void ApplyChanges(Component obj)
        {
            if (_serializedObject == null) return;
            
            _serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(obj);
        }
    }
}