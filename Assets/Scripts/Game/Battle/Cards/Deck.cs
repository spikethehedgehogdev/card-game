using System;
using System.Collections.Generic;
using Shared.Enum;
using Shared.Extensions;

namespace Game.Battle.Cards
{
    public struct DeckPreset
    {
        public int BattlerCount;
        public int UnitTypeCount;
        public int HandDrawCount;
    }
    public interface IDeck
    {
        List<ICard> GetBattlerSet(int battlerIndex);
        void ShuffleDraw();
    }
    
    public class Deck : IDeck
    {
        private readonly DeckPreset _preset;
        private readonly Dictionary<int, List<ICard>> _baseSets;
        private readonly List<ICard> _drawSet;

        public Deck(ICardFactory factory)
        {
            _preset = new DeckPreset
            {
                BattlerCount = 2,
                UnitTypeCount = Enum.GetValues(typeof(CardType)).Length,
                HandDrawCount = 3,
            };
            
            _baseSets = new Dictionary<int, List<ICard>>();
            _drawSet = new List<ICard>();
            
            for (var i = 0; i < _preset.BattlerCount; i++)
            {
                var set = new List<ICard>();
                for (var j = 0; j < _preset.UnitTypeCount; j++)
                {
                    set.Add(factory.Create(j, i));
                    for (var k = 0; k < _preset.HandDrawCount; k++) 
                        _drawSet.Add(factory.Create(j, -1));
                
                }
                _baseSets.Add(i, set);
            }
            
            _drawSet.Shuffle();
        }
        
        public List<ICard> GetBattlerSet(int battlerIndex)
        {
            var cards = new List<ICard>();
            
            cards.AddRange(_baseSets[battlerIndex]);
            var rangeIndex = _preset.HandDrawCount * battlerIndex;
            cards.AddRange(_drawSet.GetRange(rangeIndex, _preset.HandDrawCount));
            
            cards.Shuffle();
            
            return cards;
        }

        public void ShuffleDraw() => _drawSet.Shuffle();
        
    }
}