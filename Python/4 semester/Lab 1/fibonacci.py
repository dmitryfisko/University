class Fibonacci(object):
    def __init__(self, gen_limit=10):
        self._generator = self._next_fib(gen_limit)

    def __iter__(self):
        return self

    def _next_fib(self, gen_limit):
        fib = prev_fib = 1
        for _ in xrange(gen_limit):
            yield prev_fib
            prev_fib, fib = fib, prev_fib + fib

    def next(self):
        return self._generator.next()
