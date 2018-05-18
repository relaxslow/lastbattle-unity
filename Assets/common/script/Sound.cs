using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound  {
   
    public static List<Sound> All = new List<Sound> ();
    public string name;
    public AudioClip clip;
   

    public   Sound (string audioName)
    {
        
        name = audioName;
        clip = (AudioClip)Resources.Load ( "Game/audio/" + audioName, typeof ( AudioClip ) );
        if (clip == null)
            Debug.Log ( "audio init failed" );
     
        All.Add ( this );

    }
 
  

    public static void createAndPlay (string name,ContactInfo contactInfo)
    {
        Vector3 pos = Vector3.zero;
        if (contactInfo.selfVertex != null)
        {
            pos = contactInfo.selfVertex.pos;
        }
        else
        {
            pos = Math3D.AveragePos ( contactInfo.selfSurface.points );
        }
        createAndPlay ( name, pos );
     
    }
    public static void createAndPlay(string name, Vector3 pos, float volume = 1, bool loop = false )
    {
        Sound sound = findSound(name);
      
        if (sound == null)
            Debug.Log ( "can't find audio" );


        AudioObj audioObj = AudioObj.createFromPool();
     
        if (audioObj == null)
        {
            Debug.Log("no more audioObj");
            return;
        }
           
        audioObj.transform.position = pos;
        AudioSource audioSrc = audioObj.transform.GetComponent<AudioSource>();
        audioSrc.clip = sound.clip;
        audioSrc.loop = loop;
        audioSrc.volume = volume;
        audioSrc.Play ();

    }
    public static Sound findSound(string name)
    {
        Sound sound = null;
        for (int i = 0; i < All.Count; i++)
        {
            if (string.Equals ( All[i].name, name ))
            {
                sound = All[i];
            }
        }
        return sound;
    }
}

public class AudioObj:IPoolObj
{
    public static ObjPool<AudioObj> pool = new ObjPool<AudioObj>();
    public static List<AudioObj> globalAudio = new List<AudioObj>();//not Under SceneObj
    public Transform transform;
    public AudioSource audioSrc;
    public Sound sound;
    public void init(Transform objTransform, bool isAddtoScene = true){
        transform = objTransform;
        audioSrc=transform.GetComponent<AudioSource>();
        if (isAddtoScene)
            addToScene();
    }
    public void addToScene(){
        globalAudio.Add(this);
    }
    public void deleteFromScene(){
        transform.parent = Stage.offStage;
        globalAudio.Remove (this);
        pool.delete ( this );
    }
 
    public void reset(){
        audioSrc.clip = null;
        audioSrc.loop = false;
    }


    public static AudioObj createFromPool ( Transform hangToSceneObj = null )
    {
        AudioObj audioObj = pool.create ();
        if (audioObj == null)
            return null;
        if (hangToSceneObj == null)
        {
            audioObj.transform.parent = Stage.audios;
            audioObj.addToScene ();
        }
        else
        {
            audioObj.transform.position = hangToSceneObj.position;
            audioObj.transform.parent = hangToSceneObj;
        }



        return audioObj;
    }
    public void attachSound(Sound sound)
    {
        
        this .sound = sound;
        audioSrc.clip = sound.clip;
        

    }
    public void play(bool loop=false, float volume=1f)
    {
        if (audioSrc.isPlaying)
            return;
        if (audioSrc.clip == null)
        {
            Debug.Log ( "there is no sound attached" );
            return;
        }
        audioSrc.loop = loop;
        audioSrc.volume = volume;
        audioSrc.Play ();
    }
    public void stop()
    {
        if (!audioSrc.isPlaying)
            return;
        if (audioSrc.clip == null)
        {
            Debug.Log ( "there is no sound attached" );
            return;
        }
      
        audioSrc.Stop ();
    }
    
}
