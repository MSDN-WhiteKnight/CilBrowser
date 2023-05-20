import java.awt.*; 
import java.awt.event.*; 

class Window extends Frame{
 
    public static Statistics s;
    public static boolean Ready;
    public int btX=0,btY=30;
    public int btW=200,btH=50;
    public int tableX=20;
    public int cellW=50,cellH=30;
 
    Window(String s){
        super(s);
        Ready=false;
        setSize(400, 250);  
        setVisible(true);
        setLayout(null);
        Button bRun=new Button("New test");
        bRun.setBounds(btX,btY,btW,btH);
        add(bRun);
	
        bRun.addActionListener(new MyWindowAdapter());
        addWindowListener(new MyWindowAdapter());	
    }
    
    public void paint(Graphics g){ 
        int x=tableX,y=btY+btH+40,i,j,k=0;
        String str;

        if(Ready==false)return;

        for(i=0;i<10;i++){
            
            for(j=0;j<10;j++){
                str=new Integer((int)s.res[k]).toString();
                g.drawString(str,x,y);
                g.drawRect(x-20,y-cellH+5,cellW,cellH);
                k++;
                x+=cellW;
            }
            
            x=tableX;
            y+=cellH;
        }

        x=tableX+10*cellW+30;
        y=30;
        str="n";
        g.drawString(str,x,y);
        str="P(x=n)";
        g.drawString(str,x+cellW,y);
        str="(exp.)";
        g.drawString(str,x+cellW,y+10);
        str="P(x=n)";
        g.drawString(str,x+2*cellW,y);
        str="(theor.)";
        g.drawString(str,x+2*cellW,y+10);
        y+=cellH;

        for(i=0;i<=10;i++){
            str=new Integer(i).toString();
            g.drawString(str,x,y);
            str=new Float(s.p[i]).toString();
            g.drawString(str,x+cellW,y);
            str=new Float(s.p_t[i]).toString();
            g.drawString(str,x+2*cellW,y);
            y+=cellH;
        }
    }
};
