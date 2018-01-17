using System;
using System.Text;

namespace Leaf.Core.Text
{
    public static class StringGenerator
    {
        private static readonly Random Rnd = new Random();

        // добавляем прилагательное
        private static readonly string[] Adjectives = {
            "able", "active", "actual", "afraid", "alive", "all", "alone", "angry", "annual", "any", "apart", "asleep", "aware", "away", "basic", "best", "better", "big", "bitter", "boring", "both", "brave", "brief", "bright", "broad", "brown", "busy", "calm", "cheap", "civil", "clean", "clear", "cold", "common", "cool", "corner", "crazy", "cute", "dark", "dear", "decent", "deep", "direct", "dirty", "double", "drunk", "dry", "due", "each", "east", "easy", "either", "empty", "enough", "entire", "equal", "even", "every", "exact", "expert", "extra", "fair", "false", "famous", "far", "fast", "fat", "few", "final", "fine", "firm", "first", "fit", "flat", "formal", "free", "fresh", "front", "full", "fun", "funny", "future", "game", "glad", "glass", "global", "gold", "good", "grand", "great", "green", "gross", "guilty", "happy", "hard", "head", "heavy", "high", "his", "home", "honest", "horror", "hot", "huge", "hungry", "ideal", "inner", "joint", "just", "kind", "known", "large", "last", "late", "latter", "least", "left", "legal", "less", "life", "little", "living", "local", "lonely", "long", "loose", "lost", "loud", "low", "lower", "lucky", "mad", "main", "major", "male", "many", "master", "mean", "medium", "mental", "middle", "minor", "minute", "mobile", "more", "most", "mother", "much", "narrow", "nasty", "native", "nearby", "neat", "new", "next", "nice", "normal", "north", "novel", "odd", "old", "only", "other", "over", "own", "past", "plenty", "pretty", "prior", "public", "pure", "purple", "quick", "quiet", "rare", "raw", "real", "recent", "red", "remote", "rich", "right", "rough", "round", "royal", "sad", "same", "scared", "secret", "secure", "senior", "severe", "sexual", "sharp", "short", "shot", "signal", "silly", "silver", "simple", "single", "slight", "slow", "small", "smart", "smooth", "soft", "solid", "south", "spare", "square", "still", "stock", "street", "strict", "strong", "stupid", "such", "sudden", "super", "sure", "sweet", "tall", "that", "these", "thick", "thin", "think", "this", "tight", "time", "tiny", "top", "total", "tough", "trick", "true", "ugly", "unable", "unfair", "unique", "united", "upper", "upset", "used", "useful", "usual", "vast", "visual", "warm", "waste", "weak", "weekly", "weird", "west", "what", "which", "white", "whole", "wide", "wild", "wine", "winter", "wise", "wooden", "work", "worth", "wrong", "yellow"
        };

        // добавляем существительное
        private static readonly string[] Nouns = {
            "access", "act", "action", "actor", "ad", "advice", "affair", "age", "agency", "air", "amount", "answer", "apple", "area", "army", "art", "aspect", "back", "bad", "bank", "basis", "basket", "bath", "beer", "bird", "birth", "blood", "board", "boat", "body", "bonus", "book", "boss", "box", "bread", "breath", "bus", "buyer", "camera", "cancer", "car", "card", "care", "case", "cash", "cause", "cell", "chance", "cheek", "chest", "child", "church", "city", "class", "client", "coast", "coffee", "cookie", "county", "course", "cousin", "craft", "cycle", "dad", "data", "day", "dealer", "death", "debt", "demand", "depth", "design", "desk", "device", "dinner", "dirt", "disk", "dog", "drama", "drawer", "driver", "ear", "earth", "editor", "effort", "end", "energy", "engine", "entry", "error", "estate", "event", "exam", "extent", "eye", "face", "fact", "family", "farmer", "fat", "field", "figure", "film", "fire", "fish", "flight", "focus", "food", "force", "form", "frame", "fun", "future", "game", "garden", "gate", "gene", "girl", "goal", "group", "growth", "guest", "guide", "guitar", "hair", "half", "hall", "hand", "hat", "head", "health", "heart", "heat", "height", "home", "honey", "hotel", "house", "idea", "image", "income", "injury", "insect", "inside", "issue", "item", "job", "key", "kind", "king", "lab", "ladder", "lady", "lake", "law", "leader", "length", "level", "life", "light", "line", "list", "loss", "love", "mall", "man", "map", "market", "math", "matter", "meal", "meat", "media", "member", "memory", "menu", "method", "mind", "mode", "model", "mom", "moment", "money", "month", "mood", "mouse", "movie", "mud", "music", "name", "nation", "nature", "news", "night", "note", "number", "object", "office", "oil", "orange", "order", "oven", "owner", "page", "paper", "part", "people", "period", "person", "phone", "photo", "piano", "pie", "piece", "pizza", "place", "plan", "player", "poem", "poet", "poetry", "point", "police", "policy", "potato", "power", "price", "profit", "queen", "radio", "range", "rate", "ratio", "reason", "recipe", "record", "region", "rent", "result", "risk", "river", "road", "rock", "role", "rule", "safety", "salad", "salt", "sample", "scale", "scene", "school", "sector", "sense", "series", "shirt", "side", "singer", "sir", "sister", "site", "size", "skill", "soil", "son", "song", "sound", "soup", "source", "space", "speech", "speed", "sport", "state", "steak", "step", "stock", "store", "story", "stress", "studio", "study", "style", "sun", "system", "tale", "tax", "tea", "tennis", "term", "test", "thanks", "theory", "thing", "throat", "time", "tongue", "tooth", "top", "topic", "town", "trade", "truth", "two", "type", "uncle", "union", "unit", "user", "value", "video", "virus", "volume", "war", "water", "way", "wealth", "web", "week", "while", "wife", "winner", "woman", "wood", "word", "work", "worker", "world", "writer", "year", "youth"
        };

        public static string Random(bool password = false, bool numbers = true)
        {
            var word = new StringBuilder();

            string adj = Adjectives[Rnd.Next(Adjectives.Length - 1)];
            if (password)
                adj = adj.ToUpperFirst();
            word.Append(adj);

            string noun = Nouns[Rnd.Next(Nouns.Length - 1)];
            if (password)
                noun = noun.ToUpperFirst();
            word.Append(noun);

            // добавляем число в конце
            if (numbers)
                word.Append(password ? Rnd.Next(10, 99).ToString() : Rnd.Next(100, 9999).ToString());

            // возвращаем результат
            return word.ToString();
        }
    }
}
