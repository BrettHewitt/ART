   M S B A M L   `   `   `  ����Q  MAutomatedRodentTracker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullU/clr-namespace:AutomatedRodentTracker.Behaviours!AutomatedRodentTracker.Behaviours  5   	   ?    9AutomatedRodentTracker.View.BatchProcess.BatchProcessView   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   F
behaviours/clr-namespace:AutomatedRodentTracker.Behaviours     5   	     � Title$  BatchProcessView��5   	   $	��720��6"   $
��1280��6/    � WindowState$ 	Maximized=�6<   =   7AutomatedRodentTracker.Behaviours.WindowClosingBehavior   Close 5   	   ��  �� Path$ Close�   Closing 5   	   �� $ ClosingCommand� � 	Resources 5       �� 
TargetType �� Property �� Value��%�   '��       '� M     �� 5	   
   " ��6   .�5
      ��  � ��$ Stretch=�6;   5      �� 5   
   " �6   .�5      ��  � ��$ 15��6'   �� 5       � ��$ Center=�62   5      5      .��5      � 	 � RowDefinitions	 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   � $��0��6   
 � ColumnDefinitions
 5      �� 5      $��* �6#   	 5      �� 5       $
��Auto �6    �� 5!      $��* �6    �� 5"      $
��Auto �6    .+�5(      �� $��1��6   $��0��6$   $��Stretch=�6�   ��64   �� $ TgItemsSource� �� SelectedItem 6_   �� $ SelectedTgItem� �� Mode$ TwoWay=� �� View 5)      �� 5*      .P�5+      �� $
��Name��6)    �� Width$	 400��67    �� DisplayMemberBinding 6C   �� *Name+�� 5,      $	 140��6)   $��Progress��65    �� CellTemplate 5-      �� 5.   "   .R�5/   &   �  � Maximum$	 100Q�62   $	��120��6m    � Value 6@   �� $ Progress�$ OneWay=�6m   50   #   52      �� 53      $ 50��6)   $��Review��64    54      �� 55   "   .R�56   %   �� $��25��6S   $��25��6^   ��6,   �� $ ReviewCommand�.��57   )   �� $��R��6/   58   &   59   #   5;      5<      ��5>      �� 5?      " ��6   .�5@      ��  �@��
IsSelected 67   �� $ 
IsSelected�5A      5C      �� 5D      $��2��6   $��0��6&    �� Orientation$ 
Horizontal=�66   $��Center=�6O   .+�5E      �� $��Add File��6   ��6,   �� $ AddTgFileCommand��� 5F      $��
Add Folder��6   ��6.   �� $ AddTgFolderCommand��� 5G      $��Remove File��6   ��6/   �� $ RemoveTgFileCommand��� 5H      $��
Clear List��6   ��6.   �� $ ClearTgFilesCommand�5I      5J      �� 5L   
   $��1��6   �� 5N   
   $��2��6   $ 
Horizontal=�6"   $��Center=�6;   .+�5O      �� $��Set Output Folder��6   ��61   �� $ SetOutputFolderCommand��� 5P      $��Get Output Folder��6   ��61   �� $ LoadOutputFolderCommand��� 5Q      $��Process��6   ��6'   �� $ ProcessCommand��� 5S      $��
Export all��6   ��6*   �� $ ExportAllCommand��� 5T      $��Settings��6   ��6(   �� $ BatchSettingsCommand�5U      5V      5W      