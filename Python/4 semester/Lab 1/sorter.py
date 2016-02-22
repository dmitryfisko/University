from random import randrange


class Sorter(object):
    def __init__(self):
        pass

    def sort(self, data, type='quick'):
        methods = ['quick', 'merge', 'radix']

        if type in methods:
            return getattr(self, '_' + type)(data)
        else:
            raise ValueError('Unknown sort type.')

    def _merge(self, arr):
        if len(arr) <= 1:
            return arr

        res = []
        mid = len(arr) / 2
        left = self._merge(arr[:mid])
        right = self._merge(arr[mid:])

        i = j = 0
        while i < len(left) and j < len(right):
            if left[i] < right[j]:
                res += [left[i]]
                i += 1
            else:
                res += [right[j]]
                j += 1
        res += left[i:]
        res += right[j:]

        return res

    def _quick(self, arr):
        less = []
        equal = []
        greater = []

        if len(arr) > 1:
            pivot = arr[randrange(len(arr))]
            for item in arr:
                if item < pivot:
                    less.append(item)
                elif item == pivot:
                    equal.append(item)
                else:
                    greater.append(item)

            return self._quick(less) + equal + self._quick(greater)
        else:
            return arr

    @staticmethod
    def _radix(arr):
        if not all(isinstance(item, (int, long)) for item in arr):
            raise ValueError('List contain not integer')

        BASE = 10
        digit = 1
        last_digit = False

        while not last_digit:
            last_digit = True
            buckets = [list() for _ in range(BASE)]

            for item in arr:
                left_part = item / digit
                buckets[left_part % BASE].append(item)
                if left_part >= BASE:
                    last_digit = False

            arr = []
            for bucket in buckets:
                arr += bucket

            digit *= 10

        return arr



