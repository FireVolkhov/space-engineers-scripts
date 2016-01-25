//Запуск таймеров чаще раз в сек

private static int Delay = 0;  
private static int FrameDuration = 20;    
const string Timer1Name = "Таймер";  
const string Timer2Name = "Спуск";  

void Main(){  
  Delay++;
  
  IMyTimerBlock timer1 = (IMyTimerBlock)GridTerminalSystem.GetBlockWithName(Timer1Name);
  timer1.GetActionWithName("TriggerNow").Apply(timer1);
  
  if (Delay == FrameDuration){
    IMyTimerBlock timer2 = (IMyTimerBlock)GridTerminalSystem.GetBlockWithName(Timer2Name);
    timer2.GetActionWithName("TriggerNow").Apply(timer2);   
    Delay = 0;
  }
  
  if (Delay < FrameDuration){  
    return;  
  }     
}
