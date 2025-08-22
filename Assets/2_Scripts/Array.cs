using UnityEngine;

public class Array : MonoBehaviour
{

    void Start()
    {
        int[] numbers; // 정수형 배열 선언 (아직 공간은 없음)

        numbers = new int[5]; // 정수 5개를 저장할 수 있는 배열 만들기

        numbers[0] = 10; // 첫 번째 칸에 10 저장
        numbers[1] = 20; // 두 번째 칸에 20 저장
        numbers[2] = 30;
        numbers[3] = 40;
        numbers[4] = 50;

        Debug.Log(numbers[0]); // 10 출력
        Debug.Log(numbers[4]); // 50 출력

        int[] scores = { 90, 80, 70, 60, 50 }; // 5개의 값을 한 번에 넣어서 배열 만들기

        Debug.Log(numbers.Length); // 5 출력
    }



}
