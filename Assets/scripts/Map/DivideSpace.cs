using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivideSpace : MonoBehaviour
{
    public int totalWidth;//�� ��ü�� �ʺ�
    public int totalHeight;//�� ��ü�� ����
    [SerializeField]
    private int minWidth;//���ҵ� ������ �ּҳʺ�
    [SerializeField]
    private int minHeight;//���ҵ� ������ �ּҳ���

    public RectangleSpace totalspace;//�������� ���� ��

    public List<RectangleSpace> spaceList;//�������� �������� ������ ����Ʈ

    public void DivideRoom(RectangleSpace space)//���������������� ������ ���°��� spaceList�� �ִ´�
    {
        if(space.height >= minHeight*2 && space.width >= minWidth*2)//����, �ʺ� �Ѵ� 2�� �̻�
        {
            if (Random.Range(1, 2) == 1)//50%Ȯ��
            {
                RectangleSpace[] spaces = DivideHorizontal(space);//���η� �ڸ���

                DivideRoom(spaces[0]);//�������� �ٽ� DivideRoom()�Լ��� ������
                DivideRoom(spaces[1]);//�������� �ٽ� DivideRoom()�Լ��� ������
            }
            else if(Random.Range(1, 2) == 2)
            {
                RectangleSpace[] spaces = DivideVertical(space);//���η� �ڸ���

                DivideRoom(spaces[0]);//�������� �ٽ� DivideRoom()�Լ��� ������
                DivideRoom(spaces[1]);//�������� �ٽ� DivideRoom()�Լ��� ������
            }
        }
        else if(space.height < minHeight *2 && space.width >= minWidth * 2)//�ʺ� 2�� �̻�
        {
            RectangleSpace[] spaces = DivideVertical(space);//���η� �ڸ���

            DivideRoom(spaces[0]);//�������� �ٽ� DivideRoom()�Լ��� ������
            DivideRoom(spaces[1]);//�������� �ٽ� DivideRoom()�Լ��� ������
        }
        else if(space.height >= minHeight * 2 && space.width < minWidth * 2)//���̸� 2�� �̻�
        {
            RectangleSpace[] spaces = DivideHorizontal(space);//���η� �ڸ���

            DivideRoom(spaces[0]);//�������� �ٽ� DivideRoom()�Լ��� ������
            DivideRoom(spaces[1]);//�������� �ٽ� DivideRoom()�Լ��� ������
        }
        else//�Ѵ� 2�� ���� ����
        {
            spaceList.Add(space);//spaceList�� �ش� ������ ����
        }
    }

    private RectangleSpace[] DivideHorizontal(RectangleSpace space)//���Ʒ��� �ѷ� ������(���η� �ڸ���)
    {
        int newSpace1Height = minHeight + Random.Range(0, space.height - minHeight * 2 + 1);//�Ʒ��� �簢���� ����
                            //�ּҳ��� +  ����(0 ~ �ڸ����� �簢���� ���� - �ּ� ����2�� +1)
        RectangleSpace newSpace1 = new RectangleSpace(space.leftDown, space.width, newSpace1Height);//���ο� �簢�� newSpace1�� ����

        int newSpace2Height = space.height - newSpace1Height;//newSpace2Height(newSpace2�� ����) = �ڸ����� �簢���� ���� - newSpace1�� ����
        Vector2Int newSpace2LeftDown = new Vector2Int(space.leftDown.x, space.leftDown.y + newSpace1Height);//newSpace2�� ���ο� ���ʾƷ���(������)�� ����
                                    //x���� ������ �簢���� x�� �״��, y���� ������ �簢���� y���� newSpace1�� ���̸� ���Ѵ�
        RectangleSpace newSpace2 = new RectangleSpace(newSpace2LeftDown, space.width, newSpace2Height);//���ο� �簢�� newSpace2�� ����

        RectangleSpace[] spaces = new RectangleSpace[2];//spaces�迭�� �����
        spaces[0] = newSpace1;//spaces[0]�� newSpace1�� ����
        spaces[1] = newSpace2;//spaces[1]�� newSpace2�� ����
        return spaces;//spaces�� ��ȯ
    }

    private RectangleSpace[] DivideVertical(RectangleSpace space)//�翷���� �ѷ� ������(���η� �ڸ���)
    {
        int newSpace1Width = minWidth + Random.Range(0, space.width - minWidth * 2 + 1);//���ʻ簢���� �ʺ�
                           //�ּҳʺ� + ����(0 ~ �ڸ����� �簢���� �ʺ� - �ּҳʺ�2�� +1)
        RectangleSpace newSpace1 = new RectangleSpace(space.leftDown, newSpace1Width, space.height);//���ο� �簢�� newSpace1�� ����

        int newSpace2Width = space.width - newSpace1Width;//newSpace2Width(newSpace2�� �ʺ�) = �ڸ����� �簢���� �ʺ� - newSpace1�� �ʺ�
        Vector2Int newSpace2LeftDown = new Vector2Int(space.leftDown.x + newSpace1Width, space.leftDown.y);//newSpace2�� ���ο� ���ʾƷ���(������)�� ����
                                    //�ڸ����� �簢���� x���� newSpace1�� �ʺ� ���Ѵ�, y���� ������ y�� �״��
        RectangleSpace newSpace2 = new RectangleSpace(newSpace2LeftDown, newSpace2Width, space.height);//���ο� �簢�� newSpace2�� ����

        RectangleSpace[] spaces = new RectangleSpace[2];//spaces�迭�� ����
        spaces[0] = newSpace1;//spaces[0]�� newSpace1�� ����
        spaces[1] = newSpace2;//spaces[1]�� newSpace2�� ����
        return spaces;//spaces�� ��ȯ
    }
}
