import java.awt.*; 
import java.awt.event.*;

class MyWindowAdapter extends WindowAdapter implements ActionListener{
    public void windowClosing(WindowEvent ev){ 
        System.exit(0); //exit program
    }
    
    public void actionPerformed(ActionEvent ae){
        Window.s=new Statistics();
        Window.s.Run();
        Window.Ready=true;
        Program.fr.repaint();
    }
};
