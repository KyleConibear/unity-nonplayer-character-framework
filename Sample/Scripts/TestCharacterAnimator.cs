using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharacterAnimator : MonoBehaviour {
    private Animator m_Animator;

    private int m_HorizontalHash = 0;
    private int m_VerticalHash = 0;
    
    // Start is called before the first frame update
    void Start() {
        m_Animator = GetComponent<Animator>();
        m_HorizontalHash = Animator.StringToHash("Horizontal");
        m_VerticalHash = Animator.StringToHash("Vertical");
    }

    // Update is called once per frame
    void Update() {
        float xAxis = Input.GetAxis("Horizontal") * 2.32f;
        float yAxis = Input.GetAxis("Vertical") * 5.66f;
        
        m_Animator.SetFloat(m_HorizontalHash, xAxis, 0.1f, Time.deltaTime);
        m_Animator.SetFloat(m_VerticalHash, yAxis, 1.0f, Time.deltaTime);
    }
}
