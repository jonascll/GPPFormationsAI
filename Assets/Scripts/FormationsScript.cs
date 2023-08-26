using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class FormationsScript : MonoBehaviour
{
    [SerializeField] GameObject _knight;
    [SerializeField] int _ammountOfPawns = 0;
    int _rows;
     float _rowSpread;
    float _columnSpread;
    float _circleRadius;
     float _rectangleGap;
    
    bool _changedPath = false;
    int _columns;
    Vector3 _startPosition = Vector3.zero;
    bool _drawRect = false;
    Rect _rectToDraw = new Rect();
    GameObject[] _gameObjects;
    List<GameObject> _gameObjectsSelected = new();
    Rect _rectClicked = new Rect();
    Vector3 _clickedPosition = Vector3.zero;
    bool _rectangleFormation = false;
    bool _circleFormation = false;
    bool _triangleFormation = false;

    // Start is called before the first frame update
    void Start()
    {
      
        
        for (int i = 0; i < _ammountOfPawns; i++)
        {
            Vector3 position = new(Random.Range(-10f, 10f), -12.4f, Random.Range(-10f, 10f));
            Instantiate(_knight, position, transform.rotation);
        }

    }

    // Update is called once per frame
    void Update()
    {
        UIDocument document = GetComponent<UIDocument>();
        var root = document.rootVisualElement;
        SliderInt rowsValue = root.Q<SliderInt>("rows");
        _rows = rowsValue.value;
        Slider rowSpreadValue = root.Q<Slider>("rowSpread");
        _rowSpread = rowSpreadValue.value;
        Slider columnSpreadValue = root.Q<Slider>("columnSpread");
        _columnSpread = columnSpreadValue.value;
        Slider circleRadiusValue= root.Q<Slider>("circleRadius");
        _circleRadius = circleRadiusValue.value;
        Slider columnGapValue = root.Q<Slider>("columnGap");
        _rectangleGap = columnGapValue.value;
        RadioButtonGroup choice = root.Q<RadioButtonGroup>("pickFormation");
        switch(choice.value)
        {
            case 0:
                _rectangleFormation = true;
                _circleFormation = false;
                _triangleFormation = false;
                break;
            case 1:
                _rectangleFormation = false;
                _circleFormation = true;
                _triangleFormation = false;
                break;
                case 2:
                _rectangleFormation = false;
                _circleFormation = false;
                _triangleFormation = true;
                break;

            default:
                _rectangleFormation = false;
                _circleFormation = false;
                _triangleFormation = false;
                break;
        }
        _gameObjects = GameObject.FindGameObjectsWithTag("Pawn");
        if (Input.GetMouseButton(1))
        {
            _gameObjectsSelected.Clear();
            if (Input.mousePosition.x < Screen.width && Input.mousePosition.y < Screen.height)
            {
                if (_startPosition == Vector3.zero) {
                    _startPosition = Input.mousePosition;
                    _startPosition.y = _startPosition.y - (_startPosition.y - Screen.height / 2.0f) * 2.0f;
                }

                _rectToDraw = new Rect(_startPosition.x, _startPosition.y, -(_startPosition.x - Input.mousePosition.x), -(_startPosition.y - (Input.mousePosition.y - ((Input.mousePosition.y - Screen.height / 2.0f) * 2.0f))));

                _drawRect = true;
            }
        }
        if (Input.GetMouseButtonUp(1)) {
            _drawRect = false;
            ;
            foreach (GameObject gameObject in _gameObjects)
            {

                Vector3 posOfObject = Camera.main.WorldToScreenPoint(gameObject.transform.position);
                posOfObject.y = Screen.height - posOfObject.y;
                if (_rectToDraw.Contains(posOfObject, true))
                {
                    _gameObjectsSelected.Add(gameObject);
                }
            }
            float distance = 100000000000;
            GameObject closestObject = null;
            int closestIndex = -1;
            Vector3 raycastStartPoint = Camera.main.ScreenToWorldPoint(_startPosition);
            Vector3 convertedStartPoint = Vector3.zero;
            RaycastHit hit = new();
            if (Physics.Raycast(raycastStartPoint, Vector3.down * 10000, out hit))
            {
                if (hit.collider.gameObject.tag == "Level")
                {

                    convertedStartPoint = hit.collider.gameObject.transform.position;
                }
            }
            for (int i = 0; i < _gameObjectsSelected.Count; i++)
            {
                if (closestObject == null)
                {
                    closestObject = _gameObjectsSelected[i];
                    distance = Vector3.Distance(convertedStartPoint, _gameObjectsSelected[i].transform.position);
                    closestIndex = i;
                }

                if (Vector3.Distance(convertedStartPoint, _gameObjectsSelected[i].transform.position) < distance)
                {
                    closestObject = _gameObjectsSelected[i];
                    distance = Vector3.Distance(convertedStartPoint, _gameObjectsSelected[i].transform.position);
                    closestIndex = i;
                }

            }
            if (_gameObjectsSelected.Count > 0)
            {
                GameObject temp = _gameObjectsSelected[0];
                _gameObjectsSelected[0] = closestObject;
                _gameObjectsSelected[closestIndex] = temp;
            }
            _columns = (int)Mathf.Ceil((float)_gameObjectsSelected.Count / (float)_rows);
           
            _startPosition = Vector3.zero;
        }


        if (Input.GetMouseButtonDown(0))
        {

            if (Input.mousePosition.x < Screen.width && Input.mousePosition.y < Screen.height)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.tag == "Level")
                    {
                        _rectClicked = new Rect(new(Input.mousePosition.x, Screen.height - Input.mousePosition.y), new(10, 10));
                        _clickedPosition = hit.point;


                    }
                }

            }
            if(_gameObjectsSelected.Count != 0)
            {
                _columns = (int)Mathf.Ceil((float)_gameObjectsSelected.Count / (float)_rows);
            }
        }
        if (Input.GetMouseButtonDown(2))
        {
            _clickedPosition = Vector3.zero;
            _rectClicked = new Rect();
            _startPosition = Vector3.zero;
            _gameObjectsSelected.Clear();
            choice.value = -1;
        }
       
        if (_rectangleFormation)
        {
            
            if (_gameObjectsSelected.Count != 0)
            {
                int gapIndex = -1;
                if (_rectangleGap > 0)
                {
                    
                    gapIndex = _columns / 2;
                }
                for (int i = 0; i < _columns; i++)
                {
                   
                    for (int j = 0; j < _rows; j++)
                    {
                        int index = i * _rows + j;
                        if (index < _gameObjectsSelected.Count)
                        {
                            if (index != 0)
                            {

                                NavMeshAgent navMeshAgent = _gameObjectsSelected[index].GetComponent<NavMeshAgent>();
                                NavMeshPath path = new NavMeshPath();
                                if (gapIndex != -1)
                                {
                                    if (i >= gapIndex)
                                    {
                                        if (navMeshAgent.CalculatePath(_gameObjectsSelected[0].transform.position + (j * _rowSpread * -_gameObjectsSelected[0].transform.forward) + (((i * _columnSpread) + _rectangleGap )* _gameObjectsSelected[0].transform.right), path))
                                        {
                                            if (!_changedPath)
                                            {
                                                navMeshAgent.destination = _gameObjectsSelected[0].transform.position + (j * _rowSpread * -_gameObjectsSelected[0].transform.forward) + (((i * _columnSpread) + _rectangleGap) * _gameObjectsSelected[0].transform.right);
                                            }

                                        }
                                        else
                                        {
                                            _changedPath = true;
                                            NavMeshHit hit = new NavMeshHit();
                                            navMeshAgent.FindClosestEdge(out hit);
                                            navMeshAgent.destination = hit.position;
                                            if (navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
                                            {

                                                _changedPath = false;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (navMeshAgent.CalculatePath(_gameObjectsSelected[0].transform.position + (j * _rowSpread * -_gameObjectsSelected[0].transform.forward) + (i * _columnSpread * _gameObjectsSelected[0].transform.right), path))
                                        {
                                            if (!_changedPath)
                                            {
                                                navMeshAgent.destination = _gameObjectsSelected[0].transform.position + (j * _rowSpread * -_gameObjectsSelected[0].transform.forward) + (i * _columnSpread * _gameObjectsSelected[0].transform.right);
                                            }
                                        }
                                        else
                                        {
                                            _changedPath = true;
                                            NavMeshHit hit = new NavMeshHit();
                                            navMeshAgent.FindClosestEdge(out hit);
                                            navMeshAgent.destination = hit.position;
                                            if (navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
                                            {

                                                _changedPath = false;
                                            }
                                        }
                                    }




                                }
                                else
                                {
                                    if (navMeshAgent.CalculatePath(_gameObjectsSelected[0].transform.position + (j * _rowSpread * -_gameObjectsSelected[0].transform.forward) + (i * _columnSpread * _gameObjectsSelected[0].transform.right), path))
                                    {
                                        if (!_changedPath)
                                        {
                                            if (i > gapIndex && gapIndex != -1)
                                            {
                                                navMeshAgent.destination = _gameObjectsSelected[0].transform.position + (j * _rowSpread * -_gameObjectsSelected[0].transform.forward) + (i * (_columnSpread + _rectangleGap) * _gameObjectsSelected[0].transform.right);
                                            }
                                            else
                                            {
                                                navMeshAgent.destination = _gameObjectsSelected[0].transform.position + (j * _rowSpread * -_gameObjectsSelected[0].transform.forward) + (i * _columnSpread * _gameObjectsSelected[0].transform.right);
                                            }

                                        }

                                    }
                                    else
                                    {

                                        _changedPath = true;
                                        NavMeshHit hit = new NavMeshHit();
                                        navMeshAgent.FindClosestEdge(out hit);
                                        navMeshAgent.destination = hit.position;
                                        if (navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
                                        {

                                            _changedPath = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        if (_circleFormation)
        {
            if(_gameObjectsSelected.Count != 0)
            {
                float radianPerAgent =  (2.0f* Mathf.PI) / _gameObjectsSelected.Count ;
                
                float currentRadian = 0 + radianPerAgent;
                Vector2 centerCircle = new(_gameObjectsSelected[0].transform.position.x - _circleRadius, _gameObjectsSelected[0].transform.position.z);
                NavMeshPath path = new NavMeshPath();
              
                for (int i = 0; i < _gameObjectsSelected.Count; i++)
                {
                    if(i != 0)
                    {
                        NavMeshAgent agent = _gameObjectsSelected[i].GetComponent<NavMeshAgent>();
                       
                        if(agent.CalculatePath(new Vector3(centerCircle.x + (_circleRadius * Mathf.Cos(currentRadian)), _gameObjectsSelected[i].transform.position.y, centerCircle.y + (_circleRadius * Mathf.Sin(currentRadian))), path))
                        {
                            if(!_changedPath)
                            {
                                agent.destination = new Vector3(centerCircle.x + (_circleRadius * Mathf.Cos(currentRadian)), _gameObjectsSelected[i].transform.position.y, centerCircle.y + (_circleRadius * Mathf.Sin(currentRadian)));
                            }
                     
                        } else
                        {
                            if(agent.pathStatus == NavMeshPathStatus.PathComplete)
                            {
                                _changedPath = false;
                            }
                            NavMeshHit hit = new NavMeshHit();
                            agent.FindClosestEdge(out hit);
                            agent.destination = hit.position;
                        }
                    
                      
                           currentRadian += radianPerAgent;
                    
                    }
                }
            }

        }
      
        if(_clickedPosition != Vector3.zero)
        {
            
            if(_gameObjectsSelected.Count > 0)
            {
                NavMeshAgent navMeshAgent = _gameObjectsSelected[0].GetComponent<NavMeshAgent>();
                navMeshAgent.destination = _clickedPosition;
            }
   
        }
        if(_triangleFormation)
        {
            if(_gameObjectsSelected.Count != 0)
            {
            
                float columnSpread = _columnSpread / 2.0f;
                for (int i = 1; i < _gameObjectsSelected.Count; ++i)
                {
                    var n = i + 1;
                    NavMeshAgent agent =  _gameObjectsSelected[i].GetComponent<NavMeshAgent>();
                    Vector3 endPosition = new Vector3(_gameObjectsSelected[0].transform.position.x + (n - GetCountForRow(GetRow(n))) * columnSpread + ((GetRow(n) - 1) * columnSpread / 2), _gameObjectsSelected[0].transform.position.y, _gameObjectsSelected[0].transform.position.z + (GetRow(n) - 1) * -_rowSpread);
                    endPosition = _gameObjectsSelected[0].transform.rotation * (endPosition - _gameObjectsSelected[0].transform.position) + _gameObjectsSelected[0].transform.position;
                    NavMeshPath path = new NavMeshPath();
                    agent.CalculatePath(endPosition, path);
                    if(agent.pathStatus == NavMeshPathStatus.PathComplete)
                    {
                        Quaternion.Lerp(agent.transform.rotation, _gameObjectsSelected[0].transform.rotation, Time.deltaTime);
                    }
                    if (path.status != NavMeshPathStatus.PathInvalid )
                    {
                        if(!_changedPath)
                        agent.destination = endPosition;
                    } else
                    {
                        _changedPath = true;
                        NavMeshHit hit = new NavMeshHit();
                        agent.FindClosestEdge(out hit);
                        agent.destination = hit.position;
                        if(agent.pathStatus == NavMeshPathStatus.PathComplete)
                        {
                            _changedPath = false;
                        }
                    }
             
                   
                }
            }
        }
    }
    private void OnGUI()
    {
        if(_drawRect )
        {
            EditorGUI.DrawRect(_rectToDraw, new Color(0.7f, 0.7f, 0.7f, 0.1f));
        }

        foreach(GameObject gameObject in _gameObjectsSelected) {

            Vector3 posOfObject = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            posOfObject.y = Screen.height - posOfObject.y;
            EditorGUI.DrawRect(new Rect(posOfObject, new(20, 20)), new Color(0.2f,0.8f, 0.2f, 0.5f));
        }

        EditorGUI.DrawRect(_rectClicked, new Color(1, 0, 0, 1));
    }
    private int GetRow(int number)
    {
        return (int)Mathf.Ceil((Mathf.Sqrt(8 * number + 1) - 1) / 2);
    }
    private int GetCountForRow(int row)
    {
        return row * (row + 1) / 2;
    }
}
