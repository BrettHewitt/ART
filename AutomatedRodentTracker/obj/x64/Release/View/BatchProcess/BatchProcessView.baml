   M S B A M L   `   `   `  ���� Q  MAutomatedRodentTracker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullU/clr-namespace:AutomatedRodentTracker.Behaviours!AutomatedRodentTracker.Behaviours  ?    9AutomatedRodentTracker.View.BatchProcess.BatchProcessView   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     F
behaviours/clr-namespace:AutomatedRodentTracker.Behaviours       � Title$  BatchProcessView��$	��720��$
��1280�� � WindowState$ 	Maximized=�=   7AutomatedRodentTracker.Behaviours.WindowClosingBehavior   Close ��  �� Path$ Close�   Closing �� $ ClosingCommand� � 	Resources  �� 
TargetType �� Property �� Value��%y   '��       '� (     �� " ��.���  � ��$ Stretch=��� " �.���  � ��$ 15����  � ��$ Center=�.��� 	 � RowDefinitions	 �� $��* ��� $
��Auto ��� $
��Auto �.+�� $��0��
 � ColumnDefinitions
 �� $��* �	 �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��1��$��0��$��Stretch=����� $ TgItemsSource� �� SelectedItem �� $ SelectedTgItem� �� Mode$ TwoWay=� �� View �� .P��� $
��Name�� �� Width$	 400�� �� DisplayMemberBinding �� *Name+�� $	 140��$��Progress�� �� CellTemplate �� .R��  � Maximum$	 100Q�$	��120�� � Value �� $ Progress�$ OneWay=��� $ 50��$��Review�� �� .R��� $��25��$��25������ $ ReviewCommand�.���� $��R������ " ��.���  �@��
IsSelected �� $ 
IsSelected��� $��2��$��0�� �� Orientation$ 
Horizontal=�$��Center=�.+��� $��Add File������ $ AddTgFileCommand��� $��
Add Folder������ $ AddTgFolderCommand��� $��Remove File������ $ RemoveTgFileCommand��� $��
Clear List������ $ ClearTgFilesCommand��� $��1���� $��2��$ 
Horizontal=�$��Center=�.+��� $��Set Output Folder������ $ SetOutputFolderCommand��� $��Get Output Folder������ $ LoadOutputFolderCommand��� $��Process������ $ ProcessCommand��� $��
Export all������ $ ExportAllCommand��� $��Settings������ $ BatchSettingsCommand�