   M S B A M L   `   `   `  ���� Q  MAutomatedRodentTracker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullU/clr-namespace:AutomatedRodentTracker.Behaviours!AutomatedRodentTracker.Behaviours  ?    9AutomatedRodentTracker.View.BatchProcess.BatchProcessView   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     F
behaviours/clr-namespace:AutomatedRodentTracker.Behaviours       � Title$  BatchProcessView��$	��720��$
��1280�� � WindowState$ 	Maximized=�=   7AutomatedRodentTracker.Behaviours.WindowClosingBehavior   Close ��  �� Path$ Close�   Closing �� $ ClosingCommand� � 	Resources  �� 
TargetType �� Property �� Value��%y   '��       '� (     �� " ��.���  � ��$ Stretch=��� " �.���  � ��$ 15����  � ��$ Center=�.��� 	 � RowDefinitions	 �� $��* ��� $
��Auto ��� $
��Auto �.+�� $��0��
 � ColumnDefinitions
 �� $��* ��� $��* �	 �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��Load Labbook��$��2��$��0��$��2��$��Center=����� $ LoadLabbookCommand��� $��0��$��0��$��TG��$��Center=��� $��1��$��0��$��Stretch=����� $ TgItemsSource� �� SelectedItem �� $ SelectedTgItem� �� Mode$ TwoWay=� �� View �� .P��� $
��Name�� �� Width$	 200�� �� DisplayMemberBinding �� *Name+�� $��ID��$ 50�� �� *Id+�� $	��Age��$ 50�� �� CellTemplate �� .R��� ���� $	 Age�$ TwoWay=��� $��Class��$ 50�� �� $ Class��� $��Video Count��$	 100�� �� $ Videos.Count��� $	 140��$��Progress�� �� .R��  � Maximum$	 100Q�$	��120�� � Value �� $ Progress��� $ 50��$��Settings�� �� .R��� $��25��$��25������ $ SettingsCommand�.���� $��S���� $ 50��$��Review�� �� .R��� $��25��$��25������ $ ReviewCommand�.���� $��R������ " ��.���  �@��
IsSelected �� $ 
IsSelected��� $��2��$��0�� �� Orientation$ 
Horizontal=�$��Center=�.+��� $��Add File������ $ AddTgFileCommand��� $��
Add Folder������ $ AddTgFolderCommand��� $��Remove File������ $ RemoveTgFileCommand��� $��
Clear List������ $ ClearTgFilesCommand��� $��0��$��1��$	��NTG��$��Center=��� $��1��$��1������ $ NtgItemsSource� �� $ SelectedNtgItem�$ TwoWay=� �� .P��� $
��Name��$	 200�� �� *Name+�� $��ID��$ 50�� �� *Id+�� $	��Age��$ 50�� �� .R��� ���� $	 Age�$ TwoWay=��� $��Class��$ 50�� �� $ Class��� $��Video Count��$	 100�� �� $ Videos.Count��� $	 140��$��Progress�� �� .R�� $	 100Q�$	��120��$��Center=� �� $ Progress��� $ 50��$��Settings�� �� .R��� $��25��$��25������ $ SettingsCommand�.���� $��S���� $ 50��$��Review�� �� .R��� $��25��$��25������ $ ReviewCommand�.���� $��R������ " ��.���  �@��
IsSelected �� $ 
IsSelected��� $��1��$��2��$ 
Horizontal=�$��Center=�.+��� $��Add File������ $ AddNtgFileCommand��� $��
Add Folder������ $ AddNtgFolderCommand��� $��Remove File������ $ RemoveNtgFileCommand��� $��
Clear List������ $ ClearNtgFilesCommand��� $��1���� $��2��$ 
Horizontal=�$��Center=�.+��� $��Set Output Folder������ $ SetOutputFolderCommand��� $��Get Output Folder������ $ LoadOutputFolderCommand��� $��Process������ $ ProcessCommand��� $��Batch Export������ $ ExportBatchCommand��� $��
Export all������ $ ExportAllCommand��� $��Settings������ $ BatchSettingsCommand�