using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class SsimpleVisualizer : MonoBehaviour
    {
        public LlSystemGenerator Llsystem;
        List<Vector3> positions = new List<Vector3>();
        public GameObject prefab,room,line1;
        public Material lineMaterial;
        private int length = 5;
        private float angle = 90;
        public Material textureAtlas;
	    public int columnHeight = 3;
	    public static int chunkSize,chunkSizez;
	    public static Dictionary<string, Chunks> chunks;
        public Vector3 sstart,eend;
        public int Length
        {
            get
            {
                if (length > 0)
                {
                    return length;
                }
                else
                {
                    return 1;
                }
            }
            set => length = value;
        }

        private void Start()
        {
            var sequence = Llsystem.GenerateSentence();
            VisualizeSequence(sequence);
        }

        private void VisualizeSequence(string sequence)
        {
               GameObject[] Roomstart;
               Roomstart = GameObject.FindGameObjectsWithTag("Roomstart");
               foreach(GameObject room in Roomstart)
               {
            Stack<AagentParameters> savePoints = new Stack<AagentParameters>();
  
           
            var currentPosition = room.transform.position;

            Vector3 direction = Vector3.forward;
            Vector3 tempPosition = room.transform.position;

            positions.Add(currentPosition);

            foreach (var letter in sequence)
            {
                EncodingLetters encoding = (EncodingLetters)letter;
                switch (encoding)
                {
                    case EncodingLetters.save:
                        savePoints.Push(new AagentParameters
                        {
                            position = currentPosition,
                            direction = direction,
                            length = Length
                        });
                        break;
                    case EncodingLetters.load:
                        if (savePoints.Count > 0)
                        {
                            var agentParameter = savePoints.Pop();
                            currentPosition = agentParameter.position;
                            direction = agentParameter.direction;
                            Length = agentParameter.length;
                        }
                        else
                        {
                            throw new System.Exception("Dont have saved point in our stack");
                        }
                        break;
                    case EncodingLetters.draw:
                        tempPosition = currentPosition;
                        currentPosition += direction * length;
                        DrawLine(tempPosition, currentPosition, Color.red);
                        
                        positions.Add(currentPosition);
                        break;
                    case EncodingLetters.turnRight:
                        direction = Quaternion.AngleAxis(angle, Vector3.up) * direction;
                        break;
                    case EncodingLetters.turnLeft:
                        direction = Quaternion.AngleAxis(-angle, Vector3.up) * direction;
                        break;
                    default:
                        break;
                }
            }

           }
        }
        	public static string BuildChunkName(Vector3 v)
	{
		return (int)v.x + "_" + 
			         (int)v.y + "_" + 
			         (int)v.z;
	}



        private void DrawLine(Vector3 start,Vector3  end, Color color)
        {
           // Instantiate(line1, start, Quaternion.identity);
            sstart = start;
            eend = end;

             float scalex = Mathf.Abs(start.x - end.x);
             float scalez = Mathf.Abs(start.z - end.z);
             //line1.gameObject.transform.localScale = new Vector3(scalex+1,3,scalez+1);
             if(scalex==0)
             {
                  scalex++;
             }
             if (scalez==0 )
             {
                  scalez++;
             }
             chunkSize = (int) scalex ;
             chunkSizez = (int) scalez ;
             StartCoroutine(BuildChunkColumn());
             chunks = new Dictionary<string, Chunks>();
		    this.transform.position = (start+end)/2;

            
        }

        public enum EncodingLetters
        {
            unknown = '1',
            save = '[',
            load = ']',
            draw = 'F',
            turnRight = '+',
            turnLeft = '-'
        }
        	IEnumerator BuildChunkColumn()
	{
		for(int i = 0; i < 1; i++)
		{
			Vector3 chunkPosition = new Vector3(this.transform.position.x, 
												i*chunkSize, 
												this.transform.position.z);
            if(sstart.z>eend.z)
            {
            sstart.z = sstart.z-5;
            }
            if(sstart.x>eend.x)
            {
            sstart.x = sstart.x-5;
            }
			Chunks c = new Chunks(sstart, textureAtlas);

			chunks.Add(c.chunk.name, c);
		}

		foreach(KeyValuePair<string, Chunks> c in chunks)
		{
			c.Value.DrawChunk();
			yield return null;
		}
		
	}
    }
