   M S B A M L   `   `   `  ����>  :CRM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:CRMCRM  5          CRM.EmailPage   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      #localclr-namespace:CRM     5        N� Title$  	EmailPage��5	      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   &   System.Windows.Controls.DataGrid  -   5          Name$ ClientsDataGrid��5      $��1��6,   $��10q�69      AutoGenerateColumns .     SelectionMode$ Extended=�6a      Columns 5      4  .System.Windows.Controls.DataGridCheckBoxColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ Выбрать��6)      Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto 6Y   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6:   �� *
IsSelected+6Y   0  *System.Windows.Controls.DataGridTextColumn  5      $ Имя��6%    62   �� *	FirstName+  5      $ Фамилия��6%    66   �� *
SecondName+  5      $ Email��6%    64   �� *Email+5      �� 5   
   $��2��6   	 �� Orientation$	 
Horizontal=�6"   $��10q�6;   .+�5       �� $��	Тема:��6   $��Center=�6%   $	��100��6@   �� 5!      -   5        
 ��Name$
 SubjectTextBox��5!      $	��300��6.   5"      �� 5%   
   $��3��6   $	 
Horizontal=�6"   $��10q�6;   .+�5&      �� $��Текст письма:��6   $��Center=�6-   $	��100��6H   �� 5'      -   5        $
 BodyTextBox��5'      $	��300��6+   $	��100��67    �� AcceptsReturn . 6D   5(      �� 5+   
   -   5        $��4��5+      $%��Отправить письма��6   $	��150��69   $��10q�6E   $��Right=�6Q   6m   5,      5-      