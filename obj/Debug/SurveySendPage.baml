   M S B A M L   `   `   `  ����  CRM    CRM.SurveySendPage   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5        N� Title$#  Рассылка опроса��5      .��5
      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   &   System.Windows.Controls.DataGrid  -   5          Name$ ClientsDataGrid��5      $��1��6,   $
��0 10q�69      AutoGenerateColumns .     SelectionMode$ Extended=�5         CanUserAddRows .     Columns 5      4  .System.Windows.Controls.DataGridCheckBoxColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ Выбрать��6)      Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto 6Y   1  +System.Windows.Controls.DataGridBoundColumn	   Binding	 6:   �� *
IsSelected+6Y   0  *System.Windows.Controls.DataGridTextColumn  5      $ ФИО��6%   $ * 6O   	 62   �� *
FullName+6O     5      $ Email��6%   $ * 6N   	 64   �� *Email+6N   5      �� 5   
   $��2��6   $
��0 10q�6"   .+�5      �� $&�� Параметры письма:��6   $
��Bold"�61   $��0 0 0 5q�6C   �� 5!      
 �� Orientation$
 
Horizontal=�6   $	��0 5q�62   .+�5"      �� $��	Тема:��6   $	��100��6)   $��Center=�65   �� 5#      -   5         ��Name$ SubjectTextBox��5#      $	��400��62   $>��8Приглашение к участию в опросе��6>   5$      �� 5&      $
 
Horizontal=�6   $	��0 5q�62   .+�5'      �� $��Текст письма:��6   $	��100��61   $	��Top=�6=   �� 5(      -   5        $ BodyTextBox��5(      $	��400��6/   $	��100��6;    �� TextWrapping$
 Wrap=�6H    �� AcceptsReturn . $����Уважаемый {name},

Приглашаем вас принять участие в нашем опросе.
Ссылка для участия: {link}

Спасибо!��5)      5*      5+      �� 5.   
   $��3��6   $
��Wrap=�6!   $
��0 10q�65   
���������Используйте {name} для подстановки имени клиента и {link} для ссылки на опрос50      �� 53   
   $��4��6   $
 
Horizontal=�6"   $��Right=�6;   .+�54      �� -   5        $��
Назад��54      $	��100��6%   $��0 0 10 0q�61   6C   �� 55      -   5        $��Отправить��55      $	��100��6)   65   56      57      58      