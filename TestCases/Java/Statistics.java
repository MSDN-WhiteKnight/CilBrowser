import java.util.*;
import java.lang.Math.*;

public class Statistics{
    protected Random rnd;
    public byte res[];
    public float p[];
    public float p_t[];
    public static final int COUNT_TESTS=100;
    public static final int COUNT_THROWS=10;

    public Statistics(){
        float x;
        int k;

        rnd=new Random();
        res=new byte[COUNT_TESTS];
        p=new float[COUNT_THROWS+1];
        p_t=new float[COUNT_THROWS+1];
        x=(float)Math.pow(0.5f,(float)COUNT_THROWS);
        
        for(k=0;k<=COUNT_THROWS;k++){
            p_t[k]=x;
            x*=(COUNT_THROWS-k)/(k+1.0f);
        }	
    }
	
    public byte Test(){
        int i;
        byte c=0;
        float x;
        
        for(i=0;i<COUNT_THROWS;i++){
            x=rnd.nextFloat();
            if(x>=0.5f)c++;
        }
        
        return c;
    }

    public void Run(){
        int i;
        byte x;
        
        for(i=0;i<=COUNT_THROWS;i++){
            p[i]=0.0f;
        }

        for(i=0;i<COUNT_TESTS;i++){
            x=Test();
            res[i]=x;
            p[x]+=1.0f;
        }
        
        for(i=0;i<=COUNT_THROWS;i++){
            p[i]/=(float)COUNT_TESTS;
        }	
    }
};
