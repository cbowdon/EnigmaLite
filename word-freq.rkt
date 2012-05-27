#lang racket

(define text "Hello my name is John, I'm the batman in my spare time and I like to eat cabbage. The best thing about cabbage is the crunchiness.")

(define words 
  (map
   (λ (x) (string-downcase (regexp-replace ",|\\." x "")))
   (regexp-split " " text)))

(define (count ws)
  (let ([h (make-hash)])
    (let loop ([w ws])
      (if (empty? w) 
          h          
          (begin
            (if (hash-ref h (first w) #f)
                (hash-update! h (first w) add1)
                (hash-set! h (first w) 1))       
            (loop (rest w)))))))

(define h (count words))

(define sorted (sort (hash-keys h) > #:key (λ (x) (hash-ref h x))))

(for ([i (in-list sorted)])
  (printf "Freqs.Add(\"~a\",~a);~n" i (hash-ref h i)))
