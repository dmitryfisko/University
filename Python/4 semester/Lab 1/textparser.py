import re
from collections import Counter
from collections import defaultdict

class TextParser(object):
    def __init__(self, file_name='text.txt'):
        text = self._extract_text(file_name)
        self._words_stat = self._extract_words_stat(text)
        self._senten_mean, self._senten_median = \
            self._extract_sentences_stat(text)

    @staticmethod
    def _extract_text(file_name):
        try:
            with open(file_name, 'r') as f:
                return re.sub(r'[^ a-z!:;,\.\?\s]', '',
                              f.read().lower())
        except IOError:
            print 'Failed to load file {file}'.format({'file': file_name})

    @staticmethod
    def _extract_words(text):
        words = re.split('\.|\?|!|;|\s|,|:', text)
        return [word for word in words if word != '']

    def _extract_words_stat(self, text):
        words_stat = defaultdict(int)
        words = self._extract_words(text)
        for word in words:
            words_stat[word] += 1

        return words_stat

    # TODO Does 'senten' should be 'sentens'
    def _extract_sentences_stat(self, text):
        sentences = re.split('\.|\?|!|;|\s', text)
        senten_words_count = map(lambda senten: len(self._extract_words(senten)), sentences)
        senten_words_count = [count for count in senten_words_count if count != 0]
        senten_words_count.sort()

        senten_len = len(senten_words_count)
        if senten_len < 1:
            senten_mean = None
            senten_median = None
        else:
            senten_mean = sum(senten_words_count) / float(senten_len)
            senten_median = senten_words_count[senten_len / 2]

        return senten_mean, senten_median

    def top_n_grams(self, n=4, top_k=10):
        n_grams = defaultdict(int)
        for word, count in self._words_stat.iteritems():
            for i in xrange(len(word) - n + 1):
                n_grams[word[i:i + n]] += 1
        top = Counter(n_grams).most_common(top_k)
        return [item[0] for item in top]

    def sentences_mean(self):
        return self._senten_mean

    def sentences_median(self):
        return self._senten_median

    def __getitem__(self, word):
        word = word.lower()
        return self._words_stat[word]
