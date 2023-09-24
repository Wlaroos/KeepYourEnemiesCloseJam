using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TextWobble : MonoBehaviour
{
    // Variables for Word Change
    [SerializeField] private bool _charMode;
    [SerializeField] private bool _applyOnlyToSelection;
    [TextArea(5,5)]                             //make this area bigger if you need to add more lines
    [SerializeField] private string _textToChangeColor;
    private int _specificWordIndex;
    
    // Variables for Text Change
    private TMP_Text _textMesh;
    private Mesh _mesh;
    private Vector3[] _vertices;
    private List<int> _wordIndexes;
    private List<int> _wordLengths;
    
    // Wobbling Variables
    private Vector3 _offset;
    [SerializeField] private bool _wobbleText = false;
    [SerializeField] private float _wobblingSpeed = 1f;

    // Global Variables
    private TMP_Text _tempTextMesh;
    private string _tempString;

    // Color Variables
    [SerializeField] private bool _colorText = false;
    [SerializeField] private float _colorSpeed = 2f;
    public Gradient Gradient;
   
    // Fade Variables
    private float _fadeInDuration;  // Duration for fading in (in seconds)
    private float _startTime;  // Time when fading in started

    public TextWobble(TMP_Text textMesh)
    {
        _textMesh = textMesh;
    }

    // Start is called before the first frame update
    void Start()
    {
        _textMesh = GetComponent<TMP_Text>();
        _wordIndexes = new List<int>();
        _wordLengths = new List<int>();
        
        if (_applyOnlyToSelection)
        {
            char[] tempChars = _textToChangeColor.ToLower().ToCharArray();
            _tempString = _textMesh.text.ToLower();
            
            for (int i = 0; i < _tempString.Length; i++)
            {
                if(_tempString[i].CompareTo(tempChars[0]) == 0)      //check if the first letter of the Selection is found
                {
                    for (int j = 1; j < tempChars.Length; j++)      //Go over every character in our selection
                    {
                        if(_tempString[i + j].CompareTo(tempChars[j]) != 0)  //if next character is not the character in our selection, go back to the 1st For Loop
                        {
                            break;
                        }          
                        
                        if(j == tempChars.Length - 1)       //if every character was correct, We found our selection!!
                        {
                            print("Found the word!");
                            _wordIndexes.Add(i);
                            _wordLengths.Add(tempChars.Length);
                            _tempString = _textToChangeColor;                            
                            return;
                        }
                    }                    
                }
                //Our selection was not found, Script will deactivate itself because it wont do anything anyways
                if (i == _tempString.Length - 1) { print("Word not found!"); this.enabled = false; }   
            }            
        }
        else
        {
            _wordIndexes.Add(0); //wordIndexes needs to start with an index of 0
            _tempString = _textMesh.text;
            
            for (int index = _tempString.IndexOf(' '); index > -1; index = _tempString.IndexOf(' ', index + 1))
            {
                _wordLengths.Add(index - _wordIndexes[_wordIndexes.Count - 1]);
                _wordIndexes.Add(index + 1);
            }
            _wordLengths.Add(_tempString.Length - _wordIndexes[_wordIndexes.Count - 1]);
        }  
    }
    
    // Update is called once per frame
    void Update()
    {
        _textMesh.ForceMeshUpdate();
        _mesh = _textMesh.mesh;
        _vertices = _mesh.vertices;
        Color[] colors = _mesh.colors;

        if (!_charMode)
        {
            for (int w = 0; w < _wordIndexes.Count; w++)
            {
                int wordIndex = _wordIndexes[w];
                if (_wobbleText) _offset = Wobble(Time.time * _wobblingSpeed + w);

                for (int i = 0; i < _wordLengths[w]; i++)
                {
                    TMP_CharacterInfo c = _textMesh.textInfo.characterInfo[wordIndex + i];

                    int index = c.vertexIndex;

                    if (_applyOnlyToSelection)
                    {
                        if (index == 0) continue;
                    } //if there is a blank character, go to the next character instead

                    if (_colorText)
                    {
                        colors[index] = Gradient.Evaluate(Mathf.Repeat(Time.time * _colorSpeed - _vertices[index].x * 0.001f, 1f));
                        colors[index + 1] = Gradient.Evaluate(Mathf.Repeat(Time.time * _colorSpeed - _vertices[index + 1].x * 0.001f, 1f));
                        colors[index + 2] = Gradient.Evaluate(Mathf.Repeat(Time.time * _colorSpeed - _vertices[index + 2].x * 0.001f, 1f));
                        colors[index + 3] = Gradient.Evaluate(Mathf.Repeat(Time.time * _colorSpeed - _vertices[index + 3].x * 0.001f, 1f));
                    }
                    if (_wobbleText)
                    {
                        _vertices[index] += _offset;
                        _vertices[index + 1] += _offset;
                        _vertices[index + 2] += _offset;
                        _vertices[index + 3] += _offset;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < _textMesh.textInfo.characterCount; i++)
            {
                TMP_CharacterInfo c = _textMesh.textInfo.characterInfo[i];
                int index = c.vertexIndex;

                if (_wobbleText) _offset = Wobble(Time.time * _wobblingSpeed + i);
                
                if (_colorText)
                {
                    colors[index] = Gradient.Evaluate(Mathf.Repeat(Time.time * _colorSpeed - _vertices[index].x * 0.001f, 1f));
                    colors[index + 1] = Gradient.Evaluate(Mathf.Repeat(Time.time * _colorSpeed - _vertices[index + 1].x * 0.001f, 1f));
                    colors[index + 2] = Gradient.Evaluate(Mathf.Repeat(Time.time * _colorSpeed - _vertices[index + 2].x * 0.001f, 1f));
                    colors[index + 3] = Gradient.Evaluate(Mathf.Repeat(Time.time * _colorSpeed - _vertices[index + 3].x * 0.001f, 1f));
                }

                if (_wobbleText)
                {
                    _vertices[index] += _offset;
                    _vertices[index + 1] += _offset;
                    _vertices[index + 2] += _offset;
                    _vertices[index + 3] += _offset;
                }
            }
        }

        _mesh.vertices = _vertices;
        _mesh.colors = colors;
        _textMesh.canvasRenderer.SetMesh(_mesh);
    }
    
    Vector2 Wobble(float time)
    {
        return new Vector2(Mathf.Sin(time * 3.3f) / 25, Mathf.Cos(time * 2.5f) / 25);
    }
    
    // Function to scale the text (zoom in or out) over a specified duration
    public void ScaleText(Vector2 targetFontSize, float duration)
    {
        StartCoroutine(AnimateTextScaling(targetFontSize, duration));
    }
    
    // Coroutine to scale the text over a specified duration
    private IEnumerator AnimateTextScaling(Vector2 targetScale, float duration)
    {
        Vector2 initialScale = _textMesh.rectTransform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            _textMesh.rectTransform.localScale = Vector2.Lerp(initialScale, targetScale, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _textMesh.rectTransform.localScale = targetScale;  // Ensure we reach the target scale exactly
    }


}