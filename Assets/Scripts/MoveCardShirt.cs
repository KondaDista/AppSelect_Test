using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MoveCardShirt : MonoBehaviour
{
    [SerializeField] private int _startCardNumber;
    public int _cardNumber;
    [SerializeField] private List<Texture2D> _cardsInDeck;
    [SerializeField] private List<Transform> _cardsMore;
    [SerializeField] private Transform _cardObject;
    [SerializeField] private Transform _cardShirt;
    [SerializeField] private float _duration;
    [SerializeField] private float _durationDistanceMoreCard;
    [SerializeField] private Vector3 _shift;


    private void Start()
    {
        _cardNumber = _startCardNumber;
    }

    async public void AddCard()
    {
        if (_cardNumber > 0)
        {
            if (_cardNumber == 1)
                _cardShirt.DOScaleX(0, _duration)
                    .SetEase(Ease.OutSine);
            else
                OnMoveShirt(0, transform);

            await Task.Delay(TimeSpan.FromSeconds(_duration));
            OnScaleNewCard();
            MoveMoreCards();
            _cardNumber--;
        }
        else
        {
            _cardNumber = _startCardNumber;
            MoveMoreCardsToShirt();
        }
        
    }
    
    public void OnMoveShirt(int endValueScale, Transform spawnPosition)
    {
        Transform cardShirt = Instantiate(_cardShirt, spawnPosition.position, spawnPosition.rotation);
        cardShirt.parent = transform;
        cardShirt.localScale = new Vector3(1,1,1);
        
        cardShirt.DOScaleX(endValueScale, _duration)
            .SetEase(Ease.OutSine);

        Destroy(cardShirt.gameObject, _duration);
    }

    public void OnScaleNewCard()
    {
        Transform card = Instantiate(_cardObject, transform.position + _shift, transform.rotation);
        card.parent = transform;
        card.localScale = new Vector3(0,1,1);
        card.GetComponent<RawImage>().texture = _cardsInDeck[^_cardNumber];
        
        _cardsMore.Add(card);
        
        card.DOScaleX(1, _duration)
            .SetEase(Ease.OutSine);
    }

    public void MoveMoreCards()
    {
        if (_cardsMore.Count > 3)
        {
            Transform card = _cardsMore[0];
            _cardsMore.RemoveAt(0);
            
            Destroy(card.gameObject, _duration);
        }

        foreach (var card in _cardsMore)
        {
            card.DOLocalMoveX(card.localPosition.x + _durationDistanceMoreCard, _duration)
                .SetEase(Ease.OutSine);
        }
    }
    
    async public void MoveMoreCardsToShirt()
    {
        int i = 3;
        foreach (var card in _cardsMore)
        {
            i--;
            Debug.Log(i);
            card.GetComponent<RectTransform>().pivot = new Vector2(1f,0.5f);
            card.DOLocalMoveX((card.localPosition.x - _durationDistanceMoreCard * i) + _shift.x - 13, _duration)
                .SetEase(Ease.OutSine);
            
            card.DOScaleX(0, _duration)
                .SetEase(Ease.OutSine);
        }

        await Task.Delay(TimeSpan.FromSeconds(_duration));
        
        foreach (var card in _cardsMore)
        {
            OnMoveShirt(1, card);
        }
        
        _cardShirt.DOScaleX(1, _duration)
            .SetEase(Ease.OutSine);
    }
}
