   M S B A M L   `   `   `  ����Q  MAutomatedRodentTracker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullU/clr-namespace:AutomatedRodentTracker.Behaviours!AutomatedRodentTracker.Behaviours  5   	   '    !AutomatedRodentTracker.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   F
behaviours/clr-namespace:AutomatedRodentTracker.Behaviours     5   	     � Title$  Automated Rodent Tracker��5   	   $	��720��6*   $
��1280��67    � WindowState$ 	Maximized=�5   	   =   7AutomatedRodentTracker.Behaviours.WindowClosingBehavior   Closing 5   	   ��  �� Path$ ClosingCommand� � 	Resources 5       �� 
TargetType �� Property �� Value��%�   '��       '�� �     �� 5	   
   " ��6   .�5
      ��  � ��$	 100��6&   �� 5       � ��$ 30��6'   �� 5       � ��$ 5q�6'   5      �� 5   
   " ��6   .�5      ��  � ��$ 3q�6'   5      5       J� InputBindings 5      �� 5   
   
	 �� Key$	 D��6=   
 �� Command
 6   �� $ DisposeCommand�6=   .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   o� $��0��6   .@�5      m� $
��File��6   .@�5       m� $	��New��6    m� Command 6(   �� $ OpenVideoCommand�m� 5!      $
��Open��6    6)   �� $ OpenArtCommand�m� 5"      $
��Save��6    6)   �� $ SaveArtFileCommand�m� 5#      $��Batch Process��6    62   �� $ BatchProcessCommand�m� 5$      $��Centroid Width��6    63   �� $ BodyTestCommand�m� 5%      $��Validate��6    6-   �� $ ValidateCommand�5&      5'      � 5)   
   $��1��6    � ColumnDefinitions 5*      �� 5+      $��* �6#   �� 5,      $
��Auto �6#   .+�5/      �� $��Center=�6   $��Center=�61   $��0��6L   ��6\   �� $ Image��� 50      $��0��6A   $��Bottom=�6Q   $
��Left=�6l   $��16(�6�   
���  ��$��5q�6�   ��6   �� $ FrameNumberDisplay�6�   � 52      $��1��6    53      �� 54      $��* �6$   �� 55      $
��Auto �6$   .+�58      �� $��0��6?   $��5q�6L   ��6   �� $ Events� �� View 59      �� 5:      .P�5;      �� $��Action��6-    �� Width$	 120��6=    �� DisplayMemberBinding 6I   �� *Name+�� 5<      $��Object��6-   $ 75��6=    6H   �� *BoundaryName+�� 5=      $��Frame��6-   $ 50��6<    6G   �� *FrameNumber+5>      5@      �� 5A      $��1��6    �� Orientation$ 
Horizontal=�6*   $��Center=�6C   .+�5B      �� $��Export Raw Data��6   ��67   �� $ ExportRawDataCommand��� 5C      $��Export interactions��6   ��6;   �� $ ExportInteractionsCommand�5D      5E      5F      �� 5H   
   $��2��6   $��Stretch=�6    � Minimum$ 0Q�6<    �� TickFrequency$ 1Q�6l   $��5q�6�    � Maximum 6H   �� $ 
FrameCount�{�6~   �� $ SliderEnabled� � Value 6�   �� $ CurrentFrame�6�   5J      5K      